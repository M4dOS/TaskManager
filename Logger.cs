namespace TaskManager
{
    public partial class Logger : Form
    {
        DateTime start;//начало запуска программы 
        public Logger()//конструктор тела логгера 
        {
            InitializeComponent();
            start = new(DateTime.Now.Ticks);
        }
        public void button1_Click(object sender, EventArgs e)//событие по нажатию кнопки обновления логов 
        {
            string text = string.Empty;
            try
            {
                int c = 0;
                string[] a = File.ReadAllText(Directory.GetCurrentDirectory() + @"\data\" + @"logs\" +
                                           DateTime.Now.ToString("yyyy-MM-dd") + ".log").Split("--------------------");
                string lgs = a[^1];
                text = string.Empty;
                foreach (string str in lgs.Trim().Split('\n'))
                {
                    string[] time = str.Split(" : ")[0].Split(':');
                    int y = DateTime.Now.Year;
                    int M = DateTime.Now.Month;
                    int d = DateTime.Now.Day;
                    int H = int.Parse(time[0]);
                    int m = int.Parse(time[1]);
                    int S = int.Parse(time[2].Split('.')[0]);
                    int s = int.Parse(time[2].Split('.')[1]);
                    DateTime evnt = new(y, M, d, H, m, S, s);
                    if (evnt < start) { }
                    else { text += '\n' + str; c++; }
                }
                text = "Количество записей в логах: " + $"{c}        Время нового запуска: {start:HH:mm:ss.fff dd.MM.yyyy}" + text;
            }
            catch(Exception ex) { text = "Количество записей в логах: " + $"0        Время нового запуска: {start:HH:mm:ss.fff dd.MM.yyyy}" +
                           "\nЛога текущего дня не существует, перепроверьте файлы и работоспособность программы" +
                           $"\nТекущее сообщение ошибки: {ex.Message}"; }
            richTextBox1.Text = text;
        }
    }
}
