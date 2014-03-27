﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Rusys duom = new Rusys();
        Rusis duomenys = new Rusis();
        Sablonai sabl = new Sablonai();
        Elementas elem = new Elementas();
        Uzklausa problem = new Uzklausa();
        Uzklausa visoP = new Uzklausa();
        RandomiserClass Randomiser = new RandomiserClass();
        RandomElements RandomList = new RandomElements();
        int paskutinisNr = -1;
        int viso = 0;
        public Form1()
        {
            InitializeComponent();
            GetRusisDuomenys();
            GetSablonuDuomenys();
            GetJuosteliuIlgiai();
        }
        private void GetRusisDuomenys()
        {
            string location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\Rusys.txt";
            location = location.Substring(6);
            System.IO.StreamReader file = new System.IO.StreamReader(location);
            while (file.EndOfStream != true)
            {
                int galas = 0;
                string vardas = file.ReadLine();
                int tarpas = vardas.IndexOf(",", galas);
                duom.vardas.Add(vardas.Substring(0, tarpas));
                int j = 0;
                Boolean breakWhile = false;
                duomenys = new Rusis();
                while (breakWhile == false)
                {
                    string MedzioRusis = "";
                    double Pradzia = -1;
                    double Pabaiga = -1;
                    galas = vardas.IndexOf(":", tarpas);
                    MedzioRusis = vardas.Substring(tarpas + 2, galas - tarpas - 2);
                    tarpas = galas; galas = vardas.IndexOf("-", tarpas);
                    if (galas < 0)
                    {
                        galas = vardas.IndexOf(",", tarpas);
                        if (galas < 0)
                        {
                            galas = vardas.IndexOf("!", tarpas);
                            breakWhile = true;
                        }
                        Pradzia = Pabaiga = Convert.ToDouble(vardas.Substring(tarpas + 1, galas - tarpas - 1));
                        tarpas = galas;
                    }
                    else
                    {
                        Pradzia = Convert.ToDouble(vardas.Substring(tarpas + 1, galas - tarpas - 1));
                        tarpas = galas;
                        galas = vardas.IndexOf(",", tarpas);
                        if (galas < 0)
                        {
                            galas = vardas.IndexOf("!", tarpas);
                            breakWhile = true;
                        }
                        Pabaiga = Convert.ToDouble(vardas.Substring(tarpas + 1, galas - tarpas - 1));
                        tarpas = galas;
                    }
                    duomenys.pav.Add(MedzioRusis);
                    duomenys.pradzia.Add(Pradzia);
                    duomenys.pabaiga.Add(Pabaiga);
                    j++;
                }
                duom.Rus.Add(duomenys);
            }
            file.Close();
        }
        private void GetSablonuDuomenys()
        {
            sabl = new Sablonai();
            string location = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\Sablonai.txt";
            location = location.Substring(6);
            System.IO.StreamReader file = new System.IO.StreamReader(location);
            while (file.EndOfStream != true)
            {
                elem = new Elementas();
                string[] vardas = file.ReadLine().Split();
                sabl.SablonoNr.Add(int.Parse(vardas[0]));
                int i = 1;
                while (i < vardas.Length)
                {
                    elem.JuostIlgis.Add(int.Parse(vardas[i++]));
                    elem.Kiekis.Add(int.Parse(vardas[i++]));
                }
                sabl.SablonoElem.Add(elem);
            }
            file.Close();
        }
        private void GetJuosteliuIlgiai()
        {
            comboBox3.Items.Add(15);
            comboBox3.Items.Add(20);
            comboBox3.Items.Add(25);
            comboBox3.Items.Add(30);
            comboBox3.Items.Add(35);
            comboBox3.Items.Add(40);
            comboBox3.Items.Add(45);
            comboBox3.Items.Add(48);
            comboBox3.Items.Add(50);
            comboBox3.Items.Add(60);
            for (int i = 0; i < duom.Rus.Count; i++)
            {
                for (int j = 0; j < duom.Rus[i].pav.Count; j++)
                {
                    string test = duom.Rus[i].pav[j];
                    if (comboBox2.Items.IndexOf(test) < 0)
                    {
                        comboBox2.Items.Add(test);
                    }
                }
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Text == "")
            {
                richTextBox1.Text += comboBox2.SelectedItem + " ";
            }
            else
            {
                richTextBox1.Text += "\n" + comboBox2.SelectedItem + " ";
            }
            paskutinisNr = 1;
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (paskutinisNr == 1)
            {
                richTextBox1.Text += comboBox3.SelectedItem + " ";
            }
            else
            {
                richTextBox1.Text += "\n" + comboBox2.SelectedItem + " " + comboBox3.SelectedItem + " ";
            }
            paskutinisNr = 2;
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                richTextBox1.Text += textBox1.Text;
                paskutinisNr = 3;
            }
        }
        private void button1_Click(object sender, EventArgs e) // MAIN
        {
            if (richTextBox1.Text == "")
            {
                richTextBox2.Text = "Iveskite uzsakyma.";
            }
            else
            {
                button1.Enabled = false;
                richTextBox2.Text = "";
                Reset();
                GetSablonuDuomenys();
                int nr = ReadTextBox();
                if (nr != -1)
                {
                    AtrinktiSchemas();
                    BackgroundWorker bw = new BackgroundWorker();
                    bw.WorkerSupportsCancellation = true;
                    bw.WorkerReportsProgress = true;
                    bw.DoWork += new DoWorkEventHandler(bw_DoWork);
                    bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
                    bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
                    if (bw.IsBusy != true)
                    {
                        bw.RunWorkerAsync();
                    }
                    //test();
                }
                else
                {
                    richTextBox2.Text = "Blogas uzsakymo formulavimas.";
                    button1.Enabled = true;
                }
            }
        }
        private void Reset()
        {
            problem = new Uzklausa();
            visoP = new Uzklausa();
            Randomiser = new RandomiserClass();
            RandomList = new RandomElements();
            paskutinisNr = -1;
            viso = 0;
        }
        private int ReadTextBox()
        {
            string[] text = richTextBox1.Text.Split('\n');
            for (int i = 0; i < text.Length; i++)
            {
                string[] eilute = text[i].Split();
                if (eilute.Length != 3)
                {
                    return -1;
                }
                problem.tipai.Add(eilute[0]);
                int del = 0;
                if (int.TryParse(eilute[1], out del) == false)
                {
                    return -1;
                }
                problem.ilgis.Add(int.Parse(eilute[1]));
                if (int.TryParse(eilute[2], out del) == false)
                {
                    return -1;
                }
                problem.kiekis.Add(int.Parse(eilute[2]));
            }
            int NR = RastiTinkamiausiaRusi();
            return NR;
        }
        private int RastiTinkamiausiaRusi()
        {
            List<string> AtrinktiTipai = new List<string> { };
            List<int> AtrinktuSumos = new List<int> { };
            for (int i = 0; i < problem.ilgis.Count; i++)
            {
                if (AtrinktiTipai.IndexOf(problem.tipai[i]) < 0)
                {
                    AtrinktiTipai.Add(problem.tipai[i]);
                    AtrinktuSumos.Add(problem.kiekis[i]);
                }
                else
                {
                    AtrinktuSumos[AtrinktiTipai.IndexOf(problem.tipai[i])] += problem.kiekis[i];
                }
                if (visoP.ilgis.IndexOf(problem.ilgis[i]) < 0)
                {
                    visoP.ilgis.Add(problem.ilgis[i]);
                    visoP.kiekis.Add(problem.kiekis[i]);
                }
                else
                {
                    visoP.kiekis[visoP.ilgis.IndexOf(problem.ilgis[i])] += problem.kiekis[i];
                }
            }
            int suma = 0;
            for (int i = 0; i < AtrinktuSumos.Count; i++)
            {
                suma += AtrinktuSumos[i];
            }
            viso = suma;
            double santykis = Convert.ToDouble(AtrinktuSumos[0]) / suma;
            double ApvalintasSantykis = Math.Round(santykis * 100, 0);
            problem.santykis = ApvalintasSantykis;
            visoP.santykis = ApvalintasSantykis;
            List<Rusis> RusisAtrinkimui = new List<Rusis> { };
            double min = 99999; int mn = -1;
            for (int i = 0; i < duom.Rus.Count; i++)
            {
                if (duom.Rus[i].pav.Count == AtrinktiTipai.Count)
                {
                    int count = 0;
                    for (int j = 0; j < duom.Rus[i].pav.Count; j++)
                    {

                        if (AtrinktiTipai.IndexOf(duom.Rus[i].pav[j]) >= 0)
                        {
                            count++;
                        }
                    }
                    if (count == AtrinktiTipai.Count)
                    {
                        double pradzia = duom.Rus[i].pradzia[duom.Rus[i].pav.IndexOf(AtrinktiTipai[0])];
                        double pabaiga = duom.Rus[i].pabaiga[duom.Rus[i].pav.IndexOf(AtrinktiTipai[0])];
                        if (pradzia <= ApvalintasSantykis && pabaiga >= ApvalintasSantykis)
                        {
                            richTextBox2.Text += "Optimaliausias dizainas:  " + duom.vardas[i] + "\n";
                            return i;
                        }
                        else
                        {
                            if (Math.Min(pradzia - ApvalintasSantykis, ApvalintasSantykis - pabaiga) < min)
                            {
                                min = Math.Min(pradzia - ApvalintasSantykis, ApvalintasSantykis - pabaiga);
                                mn = i;
                            }
                        }
                    }
                }
            }
            if (mn != -1)
            {
                richTextBox2.Text += "Optimaliausias dizainas:  " + duom.vardas[mn] + "\n";
            }
            return mn;
        }
        private void AtrinktiSchemas()
        {
            richTextBox2.Text += "Schemos atitinkancios uzklausos parametrus:" + "\n";
            Sablonai sablonaiBeta = new Sablonai();
            for (int i = 0; i < sabl.SablonoNr.Count; i++)
            {
                int count = 0;
                for (int j = 0; j < visoP.ilgis.Count; j++)
                {
                    if (sabl.SablonoElem[i].JuostIlgis.IndexOf(visoP.ilgis[j]) >= 0)
                    {
                        count++;
                    }
                }
                if (count == sabl.SablonoElem[i].JuostIlgis.Count)
                {
                    sablonaiBeta.SablonoNr.Add(sabl.SablonoNr[i]);
                    sablonaiBeta.SablonoElem.Add(sabl.SablonoElem[i]);
                    richTextBox2.Text += sabl.SablonoNr[i] + " ";
                }
            }
            richTextBox2.Text += "\n";
            sabl = sablonaiBeta;
        }
        private void test() // veliau istrinti!!!
        {
            RandomElements RandomListFinal = new RandomElements();
            Randomiser.sablonas = sabl;
            for (int kartojimas = 0; kartojimas < int.Parse(textBox2.Text); kartojimas++)
            {
                RandomList = new RandomElements();
                NykstukuFabrikas(100000);
                Testing();
                for (int i = 0; i < 250; i++)
                {
                    CloneBest(100);
                    Testing();
                }
                for (int i = 0; i < RandomList.random.Count; i++)
                {
                    RandomListFinal.random.Add(RandomList.random[i]);
                }
            }
            RandomList = RandomListFinal;
            Testing();
        }
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            RandomElements RandomListFinal = new RandomElements();
            Randomiser.sablonas = sabl;
            for (int kartojimas = 0; kartojimas < int.Parse(textBox2.Text); kartojimas++)
            {
                worker.ReportProgress(kartojimas);
                RandomList = new RandomElements();
                NykstukuFabrikas(100000); //100000
                Testing();
                for (int i = 0; i < 250; i++)
                {
                    CloneBest(100);
                    Testing();
                }
                for (int i = 0; i < RandomList.random.Count; i++)
                {
                    RandomListFinal.random.Add(RandomList.random[i]);
                }
            }
            RandomList = RandomListFinal;
            Testing();
        }
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int kiek = e.ProgressPercentage + 1;
            label3.Text = "Pradetu ciklu skaicius: " + kiek + " / " + int.Parse(textBox2.Text);
        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Print();
            button1.Enabled = true;
        }
        private void NykstukuFabrikas(int kiekis) //ideti proporciju palaikyma (TEST)
        {
            // atimineti tikrinant
            Random r = new Random();
            for (int kiek = 0; kiek < kiekis; kiek++)
            {
                Randomiser = new RandomiserClass();
                Randomiser.sablonas = sabl;
                List<int> viso = new List<int> { };
                for (int i = 0; i < visoP.kiekis.Count; i++)
                {
                    Randomiser.suma.Add(visoP.kiekis[i]);
                    viso.Add(visoP.kiekis[i]);
                }
                for (int i = 0; i < Randomiser.sablonas.SablonoNr.Count; i++)
                {
                    int min = 99999;
                    for (int j = 0; j < Randomiser.sablonas.SablonoElem[i].JuostIlgis.Count; j++)
                    {
                        int index = visoP.ilgis.IndexOf(Randomiser.sablonas.SablonoElem[i].JuostIlgis[j]);
                        if (index != -1)
                        {
                            int dalyba = Convert.ToInt32(Math.Floor(Convert.ToDouble(viso[index]) / Convert.ToDouble(Randomiser.sablonas.SablonoElem[i].Kiekis[j]))); // visoP.kiekis
                            if (dalyba < min)
                            {
                                min = dalyba;
                            }
                        }
                    }
                    int atm = r.Next(0, min + 1);
                    Randomiser.kiekis.Add(atm); // atimti??
                    for (int j = 0; j < Randomiser.sablonas.SablonoElem[i].Kiekis.Count; j++)
                    {
                        int atimti = Randomiser.sablonas.SablonoElem[i].Kiekis[j] * atm;
                        int index = visoP.ilgis.IndexOf(Randomiser.sablonas.SablonoElem[i].JuostIlgis[j]);
                        viso[index] -= atimti;
                    }
                }
                RandomList.random.Add(Randomiser);
            }
        }
        private void Testing() // ideti tikrinima proporcijoms
        {
            RandomElements rand = new RandomElements();
            List<int> viso = new List<int> { };
            Boolean virsNulio = true;
            for (int i = 0; i < RandomList.random.Count; i++)
            {
                virsNulio = true;
                RandomList.random[i].suma.Clear();
                viso = new List<int> { };
                for (int get = 0; get < visoP.kiekis.Count; get++) // visoP.kiekis.Count
                {
                    viso.Add(visoP.kiekis[get]);
                }
                for (int j = 0; j < RandomList.random[i].sablonas.SablonoNr.Count; j++)
                {
                    for(int h = 0; h < RandomList.random[i].sablonas.SablonoElem[j].Kiekis.Count; h++) // ATIMTIS: su santykiu
                    {
                        int atimti = RandomList.random[i].sablonas.SablonoElem[j].Kiekis[h] * RandomList.random[i].kiekis[j];
                        int index = visoP.ilgis.IndexOf(RandomList.random[i].sablonas.SablonoElem[j].JuostIlgis[h]);
                        viso[index] -= atimti;
                    }
                }
                int suma = 0;
                for (int sum = 0; sum < viso.Count; sum++)
                {
                    if (viso[sum] < 0) // wtf?
                    {
                        virsNulio = false;
                    }
                    RandomList.random[i].suma.Add(viso[sum]); // + TIPAS
                    RandomList.random[i].ilgis.Add(visoP.ilgis[sum]);
                    suma += Math.Abs(viso[sum]);
                }
                RandomList.random[i].liekana = suma;
                if (virsNulio == true)
                {
                    rand.random.Add(RandomList.random[i]);
                }
            }
            RandomList.random = rand.random;
            rand = new RandomElements();
            List<int> k = new List<int> { };
            for (int i = 0; i < Math.Min(RandomList.random.Count, 5); i++)
            {
                int min = 999999999; int mn = -1;
                for (int j = 0; j < RandomList.random.Count; j++)
                {
                    if (RandomList.random[j].liekana < min && k.IndexOf(j) == -1)
                    {
                        min = RandomList.random[j].liekana;
                        mn = j;
                    }
                }
                k.Add(mn);
            }
            for (int i = 0; i < k.Count; i++)
            {
                rand.random.Add(RandomList.random[k[i]]);
            }
            RandomList = rand;
        }
        private void CloneBest(int kiekis) // perdeti pagal proporcija
        {
            Random r = new Random();
            int pradiniai = RandomList.random.Count;
            for (int i = 0; i < pradiniai; i++)
            {
                for (int kiek = 0; kiek < kiekis; kiek++)
                {
                    Randomiser = new RandomiserClass();
                    Randomiser.liekana = RandomList.random[i].liekana;
                    Randomiser.sablonas = sabl;
                    Randomiser.suma = RandomList.random[i].suma;
                    for (int j = 0; j < RandomList.random[i].ilgis.Count; j++)
                    {
                        Randomiser.ilgis.Add(RandomList.random[i].ilgis[j]);
                    }
                    for (int j = 0; j < RandomList.random[i].kiekis.Count; j++)
                    {
                        Randomiser.kiekis.Add(RandomList.random[i].kiekis[j]);
                    }
                    for (int j = 0; j < Randomiser.sablonas.SablonoElem.Count; j++)
                    {
                        for (int h = 0; h < Randomiser.sablonas.SablonoElem[j].Kiekis.Count; h++)
                        {
                            int plius = 0;
                            if (Randomiser.kiekis[j] > 1) 
                            {
                                plius = r.Next(1, 2);
                            }
                            int prideti = Randomiser.sablonas.SablonoElem[j].Kiekis[h] * plius;
                            int index = visoP.ilgis.IndexOf(Randomiser.sablonas.SablonoElem[j].JuostIlgis[h]);
                            Randomiser.suma[index] += prideti;
                            Randomiser.kiekis[j] -= plius;
                        }
                    }
                    for (int j = 0; j < Randomiser.sablonas.SablonoNr.Count; j++)
                    {
                        int min = 99999;
                        for (int h = 0; h < Randomiser.sablonas.SablonoElem[j].JuostIlgis.Count; h++)
                        {
                            int index = visoP.ilgis.IndexOf(Randomiser.sablonas.SablonoElem[j].JuostIlgis[h]);
                            if (index != -1)
                            {
                                int dalyba = Convert.ToInt32(Math.Floor(Convert.ToDouble(Randomiser.suma[index]) / Convert.ToDouble(Randomiser.sablonas.SablonoElem[j].Kiekis[h])));
                                if (dalyba < min)
                                {
                                    min = dalyba;
                                }
                            }
                        }
                        int atm = 0;
                        if (min > 0)
                        {
                            atm = r.Next(0, min + 1);
                        }
                        for (int h = 0; h < Randomiser.sablonas.SablonoElem[j].JuostIlgis.Count; h++)
                        {
                            int atimti = Randomiser.sablonas.SablonoElem[j].Kiekis[h] * atm;
                            int index = visoP.ilgis.IndexOf(Randomiser.sablonas.SablonoElem[j].JuostIlgis[h]);
                            if (Randomiser.suma[index] - atimti >= 0)
                            {
                                Randomiser.suma[index] -= atimti;
                                Randomiser.kiekis[j] += atm;                            }
                        }
                    }
                    RandomList.random.Add(Randomiser);
                }
            }
        }
        private void Print()
        {
            richTextBox2.Text += "Atsakymas: " + "\n";

            for (int i = 0; i < Math.Min(1, RandomList.random.Count); i++)
            {
                richTextBox2.Text += "Liekana isskirscius po sablonus: " + RandomList.random[i].liekana + "\n";
                for (int j = 0; j < RandomList.random[i].suma.Count; j++)
                {
                    richTextBox2.Text += RandomList.random[i].ilgis[j] + "= " + RandomList.random[i].suma[j] + ";  ";
                }
                richTextBox2.Text += "\n";
                for (int j = 0; j < RandomList.random[i].sablonas.SablonoElem.Count; j++)
                {
                    richTextBox2.Text += "Sablonas " + RandomList.random[i].sablonas.SablonoNr[j] + " panaudotas " + RandomList.random[i].kiekis[j] + "   kartu." + "\n";
                }
            }
        }
    }
}
public class Rusys
{
    public List<string> vardas = new List<string>{};
    public List<Rusis> Rus = new List<Rusis>{};
}
public class Rusis
{
    public List<string> pav = new List<string>{};
    public List<double> pradzia = new List<double> { };
    public List<double> pabaiga = new List<double> { };
}
public class Sablonai
{
    public List<int> SablonoNr = new List<int> { };
    public List<Elementas> SablonoElem = new List<Elementas> { };
}
public class Elementas
{
    public List<int> JuostIlgis = new List<int> { };
    public List<int> Kiekis = new List<int> { };
}
public class Uzklausa
{
    public double santykis;
    public List<string> tipai = new List<string>{};
    public List<int> kiekis = new List<int> { };
    public List<int> ilgis = new List<int> { };
}
public class RandomiserClass
{
    public Sablonai sablonas = new Sablonai();
    public List<int> kiekis = new List<int> { };
    public List<int> suma = new List<int> { };
    public List<int> ilgis = new List<int> { };
    public int liekana;
}
public class RandomElements
{
    public List<RandomiserClass> random = new List<RandomiserClass> { };
}