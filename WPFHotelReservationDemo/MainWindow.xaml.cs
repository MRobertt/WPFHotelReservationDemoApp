using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HotelReservationClassLibrary;

namespace WPFHotelReservationDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    //This could be done using more lists in a more elegant way
    //lists as tables to simulate a database format
    public partial class MainWindow : Window
    {

        List<object> BookedRoomData { get; set; } //list of lists not string
        List <object> CanceledBookingData { get; set; } //list of lists not string

        List<string> GuestInfo { get; set; }
        List<string> RoomInfo { get; set; }
        List<DateTime> BookingDates { get; set; }
        List<object> CanceledBookingGuestInfo { get; set; }
        ObservableCollection<string> Messages { get; set; }

        GuestModel guest;
        RoomModel room;

        string pathBookingsData = @"C:\Users\eXpert\Desktop\BookingsData.csv";
        string pathCanceledBookingData = @"C:\Users\eXpert\Desktop\CancelsData.csv";

        public MainWindow()
        {
            InitializeComponent();

            GuestInfo = new List<string>();
            RoomInfo = new List<string>();
            BookingDates = new List<DateTime>();
            CanceledBookingGuestInfo = new List<object>();

            BookedRoomData = new List<object>();
            CanceledBookingData = new List<object>();
            Messages = new ObservableCollection<string>();

            listBoxMessagesOutput.ItemsSource = Messages;
           
            guest = new GuestModel();
            room = new RoomModel();
   
        }

        private void BookRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            BookRoom();

            using StreamWriter writer = new StreamWriter(pathBookingsData, true);
            {
                //this doesnt display correctly in csv
                foreach (List<object> lists in CanceledBookingData)
                {
                    foreach (var item in lists)
                    {
                        writer.WriteLine($"{DateTime.Now}");
                        writer.Write($"{item.ToString()} | ");
                    }
                }
            }
        }

        private void BookRoom()
        {

            if (DataValidator() && BookedRoomChecker())
            {
                CaptureGuestInfo();
                CaptureRoomInfo();

                DateTime currentDay = DateTime.Now;
                DateTime? startDate = startDateInput.SelectedDate;
                DateTime? endDate = finishDateInput.SelectedDate;

                GuestInfo.Add(guest.FirstName);
                GuestInfo.Add(guest.LastName);
                GuestInfo.Add(guest.PhoneNumber);

                RoomInfo.Add(room.RoomNumber);

                BookingDates.Add(startDate.Value);
                BookingDates.Add(endDate.Value);
                //BookingDates.Add(currentDay);

                BookedRoomData.Add(GuestInfo);
                BookedRoomData.Add(RoomInfo);
                BookedRoomData.Add(BookingDates);

                BookingMessage();
                ClearInputs();
            }
        }

        private void BookingMessage()
        {
            Messages.Add($"Room successfully booked!");
        }

        private void CancelRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            CancelBooking();  

            using StreamWriter writer = new StreamWriter(pathCanceledBookingData, true);
            {
                //this doesnt display correctly in csv
                foreach (List<object> lists in CanceledBookingData)
                {
                    foreach (var item in lists)
                    {
                        writer.WriteLine($"{DateTime.Now}");
                        writer.Write($"{item.ToString()} | ");
                    }
                }
            }
        }

        private void CancelBooking()
        {

            if(DataValidator())
            {

                RemoveGuest();
                

                CancelBookingMessage();
                ClearInputs();
            }
        }

        private void RemoveGuest()
        {
            DateTime currentDay = DateTime.Now;
            DateTime startDate = startDateInput.SelectedDate.Value;
            DateTime endDate = finishDateInput.SelectedDate.Value;

            if
                (
                guest.FirstName == firstNameInput.Text &&
                guest.LastName == lastNameInput.Text &&
                guest.PhoneNumber == phoneNumberInput.Text &&

                room.RoomNumber == roomNumberInput.Text &&

                startDate == startDateInput.SelectedDate.Value &&
                endDate == finishDateInput.SelectedDate.Value

                )
            {
                CanceledBookingGuestInfo.Add(guest.FirstName);
                CanceledBookingGuestInfo.Add(guest.LastName);
                CanceledBookingGuestInfo.Add(guest.PhoneNumber);

                CanceledBookingGuestInfo.Add(room.RoomNumber);

                CanceledBookingGuestInfo.Add(startDate);
                CanceledBookingGuestInfo.Add(endDate);

                CanceledBookingData.Add(CanceledBookingGuestInfo);

                GuestInfo.Remove(guest.FirstName);
                GuestInfo.Remove(guest.LastName);
                GuestInfo.Remove(guest.PhoneNumber);

                RoomInfo.Remove(room.RoomNumber);

                BookingDates.Remove(startDate);
                BookingDates.Remove(endDate);
            }
        }

        private void CancelBookingMessage()
        {
            //a message box would be ok as well
            Messages.Add($"Cancellation successful!");
        }


        private void CaptureGuestInfo()
        {
            guest.FirstName = firstNameInput.Text;
            guest.LastName = lastNameInput.Text;
            guest.PhoneNumber = phoneNumberInput.Text;
        }

        private void CaptureRoomInfo()
        {
            room.RoomNumber = roomNumberInput.Text;
            room.RoomPrice = roomPriceInput.Text;
        }

        private bool DataValidator()
        {
            if (
                string.IsNullOrWhiteSpace(firstNameInput.Text) ||
                string.IsNullOrWhiteSpace(lastNameInput.Text) ||
                string.IsNullOrWhiteSpace(phoneNumberInput.Text) ||
                string.IsNullOrWhiteSpace(roomNumberInput.Text) ||
                string.IsNullOrWhiteSpace(roomPriceInput.Text) ||
                startDateInput.SelectedDate < DateTime.Now ||
                finishDateInput.SelectedDate < DateTime.Now ||
                !startDateInput.SelectedDate.HasValue ||
                !finishDateInput.SelectedDate.HasValue
                )
            {
                MessageBox.Show("Please fill all fields or make sure the fields are correct");
                return false;
            }
            return true;
        }

        private void ClearInputs()
        {
            firstNameInput.Text = "";
            lastNameInput.Text = "";
            phoneNumberInput.Text = "";
            roomNumberInput.Text = "";
            roomPriceInput.Text = "";
            startDateInput.SelectedDate = DateTime.MinValue.AddYears(DateTime.Now.Year -1);
            finishDateInput.SelectedDate = DateTime.MinValue.AddYears(DateTime.Now.Year -1);
        }

        private bool BookedRoomChecker()
        {

            DateTime startDate = startDateInput.SelectedDate.Value;
            DateTime endDate = finishDateInput.SelectedDate.Value;
            string roomNumber = roomNumberInput.Text;

            foreach (var date in BookingDates)
            {
                if (RoomInfo.Contains(roomNumber) && IsDateRangeOverlapping(startDate, endDate, date))
                {
                    MessageBox.Show("The room is already booked during the selected dates.");
                    return false;
                }
            }

            return true;
        }

        private bool IsDateRangeOverlapping(DateTime startDate1, DateTime endDate1, DateTime startDate2)
        {
            // Check if date range 1 is overlapping with date range 2
            return (startDate1 <= startDate2 && startDate2 <= endDate1);
        }

        

        private void CheckRoomBtn_Click(object sender, RoutedEventArgs e)
        {
            BookedRoomChecker();
        }

    }
}
