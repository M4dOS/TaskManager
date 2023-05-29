namespace TaskManager
{
    public partial class TaskM : Form
    {
        bool isDebug; //включен ли дебаг-мод? 
        public DataBase dataBase; //датабаза 
        string[] args; //аргументы командной строки 

        private void Main(string[] args)//основная программа (только здесь можно использовать свои функции и проверки) 
        {

        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////код ниже трогать запрещено//////////////////////////////////////////////////////
        public TaskM(bool isDebug, DataBase db, string[] args)//конструктор тела программы 
        {
            InitializeComponent();
            dataBase = db;
            this.isDebug = isDebug;
            this.args = args.ToArray();
            Text = db.progname + " " + db.version;
        }
        private void TaskM_Load(object sender, EventArgs e)//Load-метод формы 
        {
            dataBase.logState.Left = Right - 10;
            dataBase.logState.Top = Top - 10;
            if (isDebug) dataBase.logState.Show();

            Main(args);
        }

        /////////////////////////////////////////код выше трогать запрещено//////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
