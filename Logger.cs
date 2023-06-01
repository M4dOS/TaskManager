using Microsoft.VisualBasic.ApplicationServices;

namespace TaskManager
{
    public partial class Logger : Form
    {
        DateTime start;//начало запуска программы 
        public DataBase db;
        public Logger()//конструктор тела логгера 
        {
            InitializeComponent();
            start = new(DateTime.Now.Ticks);
            this.db = db;
        }
        public void LogUpdate()
        {
            button1_Click(null, EventArgs.Empty);
        }
        public void Tree()
        {
            button2_Click(null, EventArgs.Empty);
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
                text = $"Количество записей в логах: {c}\t\t\tВремя нового запуска: {start:HH:mm:ss.fff dd.MM.yyyy}" + text;
            }
            catch (Exception ex)
            {
                text = $"Количество записей в логах: 0\t\t\tВремя нового запуска: {start:HH:mm:ss.fff dd.MM.yyyy}" +
                           "\nЛога текущего дня не существует, перепроверьте файлы и работоспособность программы" +
                           $"\nТекущее сообщение ошибки: {ex.Message}";
            }
            richTextBox1.Text = text;
        }
        public void button2_Click(object sender, EventArgs e)//вырисовка "дерева" 
        {
            string text = string.Empty;

            text += $"Существующие пользователи: {db.users.Count}\n";
            foreach (var user in db.users)
            {
                text += $"\t{user.name} | {user.login} | ID: {user.id}\n";
                foreach (var desk in user.guest)
                {
                    text += $"\t\tID: {desk} | Доступ: зритель\n";
                }
                foreach (var desk in user.admin)
                {
                    text += $"\t\tID: {desk} | Доступ: Редактор\n";
                }
                foreach (var desk in user.guest)
                {
                    text += $"\t\tID: {desk} | Доступ: Создатель\n";
                }
            }
            text += '\n';

            text += $"Существующие доски: {db.desks.Count}\n";
            foreach (var desk in db.desks)
            {
                string t = "";
                if (desk.type == Type.Public) t = "Публичная";
                if (desk.type == Type.Public) t = "Приватная";
                else t = "Ошибка";
                text += $"\tID: {desk.id} | {desk.name} | Создатель: {desk.owner.name} | Тип доски: {t} | Количество карточек: {desk.cards.Count}\n";
                foreach (var card in desk.cards)
                {
                    string st0 = card.done ? "Сделано" : "не сделано";
                    text += $"\t\tID: {card.id} | {card.name} | Готовность: {st0} | Количество пунктов в чек-листе: {card.checkList.tasks.Count}\n";
                    int id = 0;
                    foreach (var task in card.checkList.tasks)
                    {
                        
                        string st = task.done ? "Сделано" : "не сделано";
                        text += $"\t\t\tID: {id}| {task.name} | Готовность: {st} | {task.card_id}\n";
                        id++;
                    }
                    
                }
            }

            richTextBox1.Text = text;
        }
    }
}
