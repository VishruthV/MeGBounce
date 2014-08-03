using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeGBounce
{
    public partial class Form1 : Form
    {
        private Trader myTrader = new Trader();

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            Log.Debug("Clicked on 'Start Button'");
            this.btn_Start.Enabled = false;
            mainBgw.RunWorkerAsync();
        }

        private void mainBgw_DoWork(object sender, DoWorkEventArgs e)
        {
            Trader myTrader = new Trader();
            Log.Debug("Starting Trader from BGWorker..");
            myTrader.Start();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Starting Scanning");
            List<Task<int>> rets = new List<Task<int>>();

            for (int i = 0; i < 10; i++)
            {
                rets.Add(DoSomethingAsync(i));
            }

            Console.WriteLine("Waiting for scanning to be completed");

            //await ret;// wd need to await a list of tasks?
            await Task.WhenAll(rets);

            Console.WriteLine("Async Scanning Done. Will start Calcn");
        }

        private async Task<int> DoSomethingAsync(int index)
        {
            Console.WriteLine("Started Scanning for " + index);

            await Task.Delay(5000);

            Console.WriteLine("Finished Scanning for " + index);
            return 6000;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HistoricalDataManager hd = new HistoricalDataManager();
            Krs.Ats.IBNet.Contract c = new Krs.Ats.IBNet.Contract("TCS", "NSE", Krs.Ats.IBNet.SecurityType.Stock, "INR");
            hd.SyncHistoricalData(c);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 2000; i++)
            {
                Log.Debug("Debug Test 1");
                Log.Error("Error Test 1");
                Log.Info("Info Test 1");
                Log.Warning("Warning Test 1");

                Exception ex = new Exception("Exception Test 1");
                Log.WriteExceptionLog(ex);

                System.Threading.Thread.Sleep(100);
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            DataAccessLayer d = DataAccessLayer.GetMySingletonDataAccessLayer();
            d.DumpData(Parameters.WorkingDirectory);
        }
    }
}
