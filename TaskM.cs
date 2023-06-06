namespace TaskManager
{
    public partial class TaskM : Form
    {
        bool isDebug; //включен ли дебаг-мод? 
        public DataBase db; //датабаза 
        string[] args; //аргументы командной строки 

        private void Main(string[] args)//основная программа (только здесь можно использовать свои функции и проверки) 
        {
            var u1 = db.GetDesk("m9OPI7wT");
            u1.id = "mzePI7wT";
            db.AddDesk(u1);
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////код ниже трогать запрещено//////////////////////////////////////////////////////
        public TaskM(bool isDebug, DataBase db, string[] args)//конструктор тела программы 
        {
            InitializeComponent();
            this.db = db;
            this.isDebug = isDebug;
            this.args = args.ToArray();
            Text = db.progname + " " + db.version;
        }

        private void TaskM_Shown(object sender, EventArgs e)
        {
            Main(args);
        }
        private void TaskM_Load(object sender, EventArgs e)//Load-метод формы 
        {
            db.logState.Left = Right - 10;
            db.logState.Top = Top - 10;
            if (isDebug) db.logState.Show();

            /*Main(args);*/
        }

        /////////////////////////////////////////код выше трогать запрещено//////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
