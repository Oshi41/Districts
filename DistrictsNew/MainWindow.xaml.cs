using System;
using System.Collections.Generic;
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
using DistrictsLib.Implementation.Printing.WPF;
using DistrictsLib.Legacy.JsonClasses;

namespace DistrictsNew
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FrameworkElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Control side)
            {
                var card = new Card
                {
                    Number = "Some new Numbber",
                    Doors = new List<Door>
                    {
                        new Door
                        {
                            Street = "2134234",
                            Number = 15,
                            Codes = new List<string>
                            {
                                "12345678912",
                                "sdfgsxdfhsft",
                                "k 124 k 65 78",
                            },
                            HouseNumber = "dfgdfvb",
                            Entrance = 3
                        },

                        new Door
                        {
                            Street = "dfgbvdfh",
                            Number = 1488,
                            Codes = new List<string>
                            {
                                "951984",
                                "1234",
                                "k 124 k 65 78",
                            },
                            HouseNumber = "NUMBEEEER",
                            Entrance = 3
                        },

                        new Door
                        {
                            Street = "FFFFFFFF",
                            Number = 1,
                            Codes = new List<string>
                            {
                                "951984",
                                "1234",
                                "k 124 k 65 78",
                            },
                            HouseNumber = "NUMBEEEER",
                            Entrance = 15
                        },
                    },

                };

                var list = new List<Card>
                {
                    card,
                    new Card
                    {
                        Number = "2",
                        Doors = new List<Door>
                        {
                            new Door
                            {
                                Street = "2134234",
                                Number = 15,
                                Codes = new List<string>
                                {
                                    "12345678912",
                                    "sdfgsxdfhsft",
                                    "k 124 k 65 78",
                                },
                                HouseNumber = "dfgdfvb",
                                Entrance = 3
                            },

                            new Door
                            {
                                Street = "dfgbvdfh",
                                Number = 1488,
                                Codes = new List<string>
                                {
                                    "951984",
                                    "1234",
                                    "k 124 k 65 78",
                                },
                                HouseNumber = "NUMBEEEER",
                                Entrance = 3
                            },

                            new Door
                            {
                                Street = "FFFFFFFF",
                                Number = 1,
                                Codes = new List<string>
                                {
                                    "951984",
                                    "1234",
                                    "k 124 k 65 78",
                                },
                                HouseNumber = "NUMBEEEER",
                                Entrance = 15
                            },
                        },

                    },
                    new Card
                    {
                        Number = "3",
                        Doors = new List<Door>
                        {
                            new Door
                            {
                                Street = "2134234",
                                Number = 15,
                                Codes = new List<string>
                                {
                                    "12345678912",
                                    "sdfgsxdfhsft",
                                    "k 124 k 65 78",
                                },
                                HouseNumber = "dfgdfvb",
                                Entrance = 3
                            },

                            new Door
                            {
                                Street = "dfgbvdfh",
                                Number = 1488,
                                Codes = new List<string>
                                {
                                    "951984",
                                    "1234",
                                    "k 124 k 65 78",
                                },
                                HouseNumber = "NUMBEEEER",
                                Entrance = 3
                            },

                            new Door
                            {
                                Street = "FFFFFFFF",
                                Number = 1,
                                Codes = new List<string>
                                {
                                    "951984",
                                    "1234",
                                    "k 124 k 65 78",
                                },
                                HouseNumber = "NUMBEEEER",
                                Entrance = 15
                            },
                        },

                    },
                    new Card
                    {
                        Number = "4",
                        Doors = new List<Door>
                        {
                            new Door
                            {
                                Street = "2134234",
                                Number = 15,
                                Codes = new List<string>
                                {
                                    "12345678912",
                                    "sdfgsxdfhsft",
                                    "k 124 k 65 78",
                                },
                                HouseNumber = "dfgdfvb",
                                Entrance = 3
                            },

                            new Door
                            {
                                Street = "dfgbvdfh",
                                Number = 1488,
                                Codes = new List<string>
                                {
                                    "951984",
                                    "1234",
                                    "k 124 k 65 78",
                                },
                                HouseNumber = "NUMBEEEER",
                                Entrance = 3
                            },

                            new Door
                            {
                                Street = "FFFFFFFF",
                                Number = 1,
                                Codes = new List<string>
                                {
                                    "951984",
                                    "1234",
                                    "k 124 k 65 78",
                                },
                                HouseNumber = "NUMBEEEER",
                                Entrance = 15
                            },
                        },

                    },
                };

                side.DataContext = list;

            }
        }
    }
}
