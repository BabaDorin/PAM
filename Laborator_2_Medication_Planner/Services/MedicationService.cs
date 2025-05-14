using Laborator_2_Medication_Planner.Models;
using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;
using System.Xml.Serialization;

public class MedicationService
{
    private readonly string FilePath = Path.Combine(FileSystem.AppDataDirectory, "medications.xml");

    public List<Medication> Medications { get; private set; } = new();

    public MedicationService()
    {
        Load();
        LocalNotificationCenter.Current.NotificationReceived += OnNotificationReceived;
    }

    public void AddMedication(Medication medication)
    {
        Medications.Add(medication);
        Save();
        ScheduleNotification(medication);
    }

    public void AddMedicationSeries(Medication medication, int days)
    {
        for (int i = 0; i < days; i++)
        {
            var medCopy = new Medication
            {
                Name = medication.Name,
                Dosage = medication.Dosage,
                Time = medication.Time.AddDays(i)
            };
            Medications.Add(medCopy);
            ScheduleNotification(medCopy);
        }
        Save();
    }

    public void Save()
    {
        using var stream = File.Create(FilePath);
        var serializer = new XmlSerializer(typeof(List<Medication>));
        serializer.Serialize(stream, Medications);
    }

    private void Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                using var stream = File.OpenRead(FilePath);
                var serializer = new XmlSerializer(typeof(List<Medication>));
                Medications = (List<Medication>)serializer.Deserialize(stream)!;
            }
        }
        catch
        {
            Medications = new();
        }

        SortMedicationsByTime();
    }

    private void SortMedicationsByTime()
    {
        Medications = Medications.OrderBy(m => m.Time).ToList();
    }

    private void ScheduleNotification(Medication medication)
    {
        var request = new NotificationRequest
        {
            NotificationId = new Random().Next(1000, 9999),
            Title = "Medication Reminder",
            Description = $"Take {medication.Name} - {medication.Dosage}",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = medication.Time,
                RepeatType = NotificationRepeat.No,
                //NotifyAutoCancelTime = TimeSpan.FromMinutes(5)
            },
            ReturningData = medication.Name
        };

        LocalNotificationCenter.Current.Show(request).Wait();
    }

    private void OnNotificationReceived(NotificationEventArgs e)
    {
        string? name = e.Request.ReturningData;
        if (!string.IsNullOrEmpty(name))
        {
            var item = Medications.FirstOrDefault(m => m.Name == name);
            if (item != null)
            {
                Medications.Remove(item);
                Save();
            }
        }
    }
}