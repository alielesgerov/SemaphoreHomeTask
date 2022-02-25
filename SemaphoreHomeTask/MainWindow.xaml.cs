using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace SemaphoreHomeTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static List<Thread> _threads = new List<Thread>();
        private static int _count;
        private static Semaphore _semaphore;
        private static int _semaphoreValue;

        public MainWindow()
        {
            InitializeComponent();
            SemaphoreNud.Minimum = 1;
            //SemaphoreSet(1);
        }

        private void WorkingThread(Semaphore semaphore)
        {
            bool st = false;

            while (!st)
            {
                if (_semaphore.WaitOne(1))
                {
                    try
                    {
                        var t = Thread.CurrentThread;
                        Dispatcher.Invoke(() => ListBoxWorkingThreads.Items.Add(t));
                        Dispatcher.Invoke(() => ListBoxWaitingThreads.Items.Remove(t));
                        Thread.Sleep(5000);
                    }
                    finally
                    {
                        st = true;
                        var t = Thread.CurrentThread;
                        Dispatcher.Invoke(() => ListBoxWorkingThreads.Items.Remove(t));
                        Dispatcher.Invoke(() => ListBoxWaitingThreads.Items.Remove(t));
                        _semaphore.Release();
                    }
                }
                else
                {
                    var t = Thread.CurrentThread;
                    Dispatcher.Invoke(() =>
                    {
                        if (!ListBoxWaitingThreads.Items.Contains(t) && !ListBoxWorkingThreads.Items.Contains(t))
                            ListBoxWaitingThreads.Items.Add(t);
                    });

                }

            }
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            _semaphore = new Semaphore(_semaphoreValue, _semaphoreValue, "Semaphore");
            _threads.Add(new Thread(_ => WorkingThread(_semaphore)));
            _threads[_count].Name = $@"Thread {_count+1}";
            ListBoxCreatedThreads.Items.Add(_threads[_count]);
            _count++;
        }


        private void ListBoxCreatedThreads_DoubleClick(object sender, EventArgs e)
        {
            if (ListBoxCreatedThreads.SelectedItem != null)
            {
                try
                {
                    var t = ListBoxCreatedThreads.SelectedItem as Thread;
                    t.Start();
                    ListBoxCreatedThreads.Items.Remove(t);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,@"Error",MessageBoxButton.OK,MessageBoxImage.Error);
                }
            }
        }

        private void SemaphoreNud_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

            if (SemaphoreNud.Value <= 0)
            {
                MessageBox.Show("Error");
                SemaphoreNud.Value = 1;
            }
            else
            {
                _semaphoreValue = Convert.ToInt32(SemaphoreNud.Value);
            }
        }
    }
}
