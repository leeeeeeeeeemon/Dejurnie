using MongoDB.Bson;
using MongoDB.Driver;
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

namespace Dejurnie
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Student
        {
            public Student(string name, string surname, int group)
            {
                Name = name;
                Surname = surname;
                Group = group;
            }
            public ObjectId _id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public int Group { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();

        }

        private async    void addToMongoButton_Click(object sender, RoutedEventArgs e)
        {
            if(RegBoxName.Text != null && RegBoxFam.Text != null)
            {
                Student newStud = new Student(RegBoxName.Text, RegBoxFam.Text, Convert.ToInt32(RegBoxGroup.Text));
                await AddToMongo(newStud);
                RegBoxName.Text = null;
                RegBoxFam.Text = null;
                RegBoxGroup.Text = null;

            }
        }

        public static async Task AddToMongo(Student st)
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("320GroupDejurnie");
            var collection = database.GetCollection<BsonDocument>("Students");
            var stud = new BsonDocument
            {
                {"Name", st.Name},
                {"Surname", st.Surname },
                {"Group", st.Group }
            };
            collection.InsertOne(stud);
        }

        private async void getDejurniyButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                var client = new MongoClient("mongodb://localhost:27017");
                var database = client.GetDatabase("320GroupDejurnie");
                var collection = database.GetCollection<Student>("Students");

                var filter = new BsonDocument("Group", Convert.ToInt32(GroupToChoise.Text));
                var list = await collection.Find(filter).ToListAsync();

                if (list.Count >= 2)
                {
                    Random rnd = new Random();
                    int firstDejNumber = rnd.Next(1, list.Count + 1);
                    int secondDejNumber = firstDejNumber;
                    while (secondDejNumber == firstDejNumber)
                    {
                        secondDejNumber = rnd.Next(1, list.Count + 1);
                    }

                    int count = 1;
                    bool fillfirst = false;
                    foreach (var item in list)
                    {
                        if ((count == firstDejNumber || count == secondDejNumber))
                        {
                            if (!fillfirst)
                            {
                                FirstDejyrnii.Text = item.Name + " " + item.Surname;
                                fillfirst = true;
                            }
                            else
                            {
                                SecondDejurniy.Text = item.Name + " " + item.Surname;
                            }

                        }
                        count++;
                    }
                }
                else
                {
                    MessageBox.Show("В группе слишком мало человек!!!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            

            
        }
    }
}
