using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NewProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Data.Data data;
        public ObservableCollection<Pair> Pairs { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            data = new Data.Data();
            Pairs = new ObservableCollection<Pair>();
            this.DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DepartmentCountLabel.Content = data.Departments.Count().ToString();
            ManagerCountLabel.Content = data.Managers.Count().ToString();
            // predicate -> sql query
            TopChiefsCountLabel.Content = data.Managers.Where(manager => manager.IdChief == null).Count().ToString();
            SubordinatesCountLabel.Content = data.Managers.Where(manager => manager.IdChief != null).Count().ToString();
            ITDepartmentCountLabel.Content = data.Managers.Where(manager => manager.IdMainDep == new Guid("D3C376E4-BCE3-4D85-ABA4-E3CF49612C94")).Count().ToString();
            TwoDepartmentsCountLabel.Content = data.Managers.Where(manager => manager.IdMainDep != null && manager.IdSecDep != null).Count().ToString();


        }

        public class Pair
        {
            public String Key { get; set; } = null!;
            public String? Value { get; set; }
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            Pairs.Clear();
            var Query = data.Managers.Where(m => m.IdMainDep == Guid.Parse("131ef84b-f06e-494b-848f-bb4bc0604266")).Select(m => new Pair() {Key = m.Surname, Value = $"{m.Name[0]}. {m.Secname[0]}"});
            foreach(var pair in Query) 
            {
                Pairs.Add(pair); // zvernenya do bd
            }
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Pairs.Clear();
            var Query = data.Managers.Join(data.Departments, m => m.IdMainDep, d => d.Id, (m, d) => new Pair() { Key = $"{m.Surname} {m.Name[0]}. {m.Secname[0]}.", Value = d.Name }).Skip(3).Take(10);
            foreach(var pair in Query)
            {
                Pairs.Add(pair);
            }
        }

        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            Pairs.Clear();
            var Query = data.Managers.Join(data.Managers, m1 => m1.IdChief, m2 => m2.Id, (m1, m2) => new Pair() { Key = $"{m1.Surname} {m1.Name[0]}. {m1.Secname[0]}.", Value = $"{m2.Surname} {m2.Name[0]}. {m2.Secname[0]}." }).Take(10).ToList().OrderBy(pair => pair.Key); // zapuskaet zapros i vydaet kolekciyu
            foreach(var pair in Query)
            {
                Pairs.Add(pair);
            }
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            Pairs.Clear();
            var Query = data.Managers.Join(data.Managers, m1 => m1.Id, m2 => m2.Id, (m1, m2) => new Pair() { Key = $"{m1.Surname} {m1.Name[0]}. {m1.Secname[0]}.", Value = m2.CreateDt.ToString() }).Take(7).ToList().OrderByDescending(pair => DateTime.Parse(pair.Value));
            foreach (var pair in Query)
            {
                Pairs.Add(pair);
            }
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            Pairs.Clear();
            var Query = data.Managers.Join(data.Departments, m => m.IdSecDep, d => d.Id, (m, d) => new Pair() { Key = $"{m.Surname} {m.Name[0]}. {m.Secname[0]}.", Value = $"{d.Name}" }).Take(10).ToList().OrderBy(pair => pair.Value);
            foreach (var pair in Query)
            {
                Pairs.Add(pair);
            }
        }
        #region Generator
        private int _N;
        public int N { get => _N++; set => _N = value; }
        #endregion
        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            // Wrong 
            N = 1;
            var Query = data.Departments.Select(d => new Pair() { Key = (N).ToString(), Value = d.Name }) ; // reas N once at begining
            Pairs.Clear();
            foreach (var pair in Query)
            {
                Pairs.Add(pair);
            }
            /*
             * var Query = rule for SQL query
             * SELECT 1, d.Name FROM Departments d ORDER BY d.Name
             * It will see as N as (1)
             */
        }

        private void Button7_Click(object sender, RoutedEventArgs e)
        {
            N = 1;
            var Query = data
                .Departments
                .OrderBy(d => d.Name)
                .AsEnumerable() // to Linq Enumerable, also ToList() can be used
                .Select(d => new Pair() { Key = (N).ToString(), Value = d.Name }); // N grows
            Pairs.Clear();
            foreach (var pair in Query)
            {
                Pairs.Add(pair);
            }
        }

        private void Button8_Click(object sender, RoutedEventArgs e)
        {
            /*
             GroupJoin - analog GROUP BY
            Nazwa widdilu - kilkist spivrobitnykiw
             */
            var Query = data.Departments.GroupJoin(data.Managers, d => d.Id, m => m.IdMainDep, (d,m) => new Pair() { Key = d.Name, Value = m.Count().ToString()} );
            Pairs.Clear();
            foreach (var pair in Query)
            {
                Pairs.Add(pair);
            }
        }

        private void Button9_Click(object sender, RoutedEventArgs e)
        {
            var Query = data.Managers.GroupJoin(data.Managers, chef => chef.Id, d => d.IdChief, (chef, d) => new Pair() { Key = $"{chef.Surname} {chef.Name[0]}. {chef.Secname[0]}.", Value = d.Count().ToString() }).Where( p => p.Value != "0");
            Pairs.Clear();
            foreach(var pair in Query)
            {
                Pairs.Add(pair);
            }
        }

        private void Button10_Click(object sender, RoutedEventArgs e)
        {
            var Query = data.Managers.GroupJoin(data.Managers, m1 => m1.Surname, m2 => m2.Surname, (m1, m2) => new Pair() { Key = m1.Surname, Value = m2.Count().ToString() }).Where(p => p.Value != "0" && p.Value != "1").Distinct();
            Pairs.Clear();
            foreach( var pair in Query)
            {
                Pairs.Add(pair);
            }
        }

        private void Button11_Click(object sender, RoutedEventArgs e)
        {
            // Назва відділу -- кількість сумісників (SecDep)
            var Query = data.Departments.GroupJoin(data.Managers, d => d.Id, m => m.IdSecDep, (d,m) => new Pair() { Key = d.Name, Value = m.Count().ToString()});
            Pairs.Clear();
            foreach( var pair in Query)
            {
                Pairs.Add(pair);
            }
        }

        private void Button12_Click(object sender, RoutedEventArgs e)
        {
            N = 1;
            var Query = data.Managers
                .GroupBy(m => m.Surname)
                .AsEnumerable()
                .Where(m => m.Count() > 1)
                .Select(m => new Pair() { Key = (N).ToString(), Value = m.Key });
            Pairs.Clear();
            foreach (var pair in Query)
            {
                Pairs.Add(pair);
            }
        }

        private void Button13_Click(object sender, RoutedEventArgs e)
        {
            /*
             * Вивести трьох співробітників з найбільшою кількістю підлеглих 
             *  к-сть підлеглих --- П.І.Б.
             */
            var Query = data
                .Managers
                .GroupJoin(data.Managers, chef => chef.Id, d => d.IdChief, (chef, d) => new Pair() { Key = d.Count().ToString(), Value = $"{chef.Surname} {chef.Name[0]}. {chef.Secname[0]}."})
                .ToList()
                .OrderByDescending(pair => int.Parse(pair.Key))
                .Take(3);
            Pairs.Clear();
            foreach (var pair in Query)
            {
                Pairs.Add(pair);
            }
        }

        private void Button14_Click(object sender, RoutedEventArgs e)
        {
            // work with navigation properties
            // worker - department
            var query = data
                .Managers
                .Select(m => new Pair() { Key = m.Surname, Value = m.MainDep.Name}); // add navigation property
            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        }

        private void NavButton1_Click(object sender, RoutedEventArgs e)
        {
            var query = data
                            .Managers
                            .Include(m => m.SecDep) // include navigation property
                            .Select(m => new Pair() { Key = m.Surname, Value = m.SecDep.Name == null ? "--": m.SecDep.Name }); // add navigation property
            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        }

        private void NavChefButton_Click(object sender, RoutedEventArgs e)
        {
            var query = data
                            .Managers
                            .Include(m => m.Chef) // include navigation property
                            .Select(m => new Pair() { Key = $"{m.Surname} {m.Name[0]}. {m.Secname[0]}.", Value = m.Chef.Name == null ? "--" : $"{m.Chef.Surname} {m.Chef.Name[0]}. {m.Chef.Secname[0]}." });
            Pairs.Clear();
            foreach (var pair in query)
            {
                Pairs.Add(pair);
            }
        }
    }
}
