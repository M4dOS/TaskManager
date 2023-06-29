using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            //db.AddDesk(u1);
            db.ShowCards(u1);
            List<Card> cards = db.CardsReturn(u1);

            ComboBox comboBoxDesks = new ComboBox();
            comboBoxDesks.Location = new Point(160, 10);
            foreach (Desk desk in db.desks)
            {
                comboBoxDesks.Items.Add(desk.name);
            }
            void comboBox_SelectedIndexChanged(object sender, EventArgs e)
            {
                Desk selectedDesk = (Desk)comboBoxDesks.SelectedItem;
                LoadDesk(selectedDesk);
                comboBoxDesks.SelectedIndexChanged += comboBox_SelectedIndexChanged;

            }

            //вся задняя панель (с чекбоксами)
            PictureBox upperPanel = new PictureBox();
            upperPanel.BackColor = Color.FromArgb(0, 0, 64);
            upperPanel.BorderStyle = BorderStyle.None;
            upperPanel.Width = 1000;
            upperPanel.Height = 35;
            upperPanel.Location = new Point(0, 0);
            upperPanel.BringToFront();

            this.Controls.Add(comboBoxDesks);
            this.Controls.Add(upperPanel);

            //отображение автора
            Label labelParticipants = new Label();
            labelParticipants.Text = "creator: " + u1.owner.name;
            labelParticipants.Width = 200;
            labelParticipants.Height = 50;
            labelParticipants.Location = new Point(700, 40);
            labelParticipants.BackColor = Color.Orange;
            labelParticipants.Font = new Font("OCR-A BT", 16, labelParticipants.Font.Style);
            labelParticipants.ForeColor = Color.MidnightBlue;
            labelParticipants.BringToFront();

            //показ названия доски
            Label labelDeskName = new Label();
            labelDeskName.Text = u1.name;
            labelDeskName.Width = 900;
            labelDeskName.Height = 50;
            labelDeskName.Location = new Point(10, 40);
            labelDeskName.BackColor = Color.Orange;
            labelDeskName.Font = new Font("OCR-A BT", 18, labelDeskName.Font.Style);
            labelDeskName.ForeColor = Color.MidnightBlue;
            labelDeskName.BringToFront();

            List<Button> buttonsCards = new();
            List<PictureBox> picturesUnderCards = new();
            List<Button> buttonsRedactCards = new();
            List<Button> buttonsAddTask = new();
            List<Button> buttonsDelCards = new();
            List<Label> lblParticipantsCards = new();
            List<Label> lblDescriptionCards = new();
            List<PictureBox> backgroundCards = new();

            int buttonWidthCard = 150;
            int buttonHeightCard = 30;
            int spacingCard = 10;
            int xCard = 10;
            int yCard = 100;

            foreach (Card card in cards)
            {
                db.CountParticipants(card);

                //кнопка с названием карточки
                Button button = new Button();
                button.Text = card.name;
                button.Font = new Font("OCR-A BT", 9, button.Font.Style);
                button.ForeColor = Color.MidnightBlue;
                button.Width = buttonWidthCard;
                button.Height = buttonHeightCard;
                button.Location = new Point(xCard, yCard);
                button.FlatStyle = FlatStyle.Flat;
                button.BringToFront();
                button.BackColor = Color.Orange;
                button.FlatAppearance.BorderSize = 0;

                //кнопка редактирования карточки
                Button btnRedactCards = new Button();
                btnRedactCards.Text = "rename";
                btnRedactCards.Font = new Font("OCR-A BT", 8, btnRedactCards.Font.Style);
                btnRedactCards.ForeColor = Color.MidnightBlue;
                btnRedactCards.FlatAppearance.BorderSize = 0;
                btnRedactCards.Width = 55;
                btnRedactCards.Height = 20;
                btnRedactCards.Location = new Point(xCard + 6, yCard + 35 - 2);
                btnRedactCards.FlatStyle = FlatStyle.Flat;
                btnRedactCards.BringToFront();
                btnRedactCards.BackColor = Color.Turquoise;

                btnRedactCards.Click += new EventHandler(btnRedactCards_Click);
                void btnRedactCards_Click(object sender, EventArgs e)
                {
                    string newButtonText = Microsoft.VisualBasic.Interaction.InputBox("Введите новое название кнопки:", "Переименование кнопки", button.Text);
                    db.ChangeNameCard(card, newButtonText);
                    button.Text = newButtonText;
                }

                //кнопка удаления карточки
                Button btnDelCards = new Button();
                btnDelCards.Text = "delete";
                btnDelCards.Font = new Font("OCR-A BT", 8, btnRedactCards.Font.Style);
                btnDelCards.ForeColor = Color.MidnightBlue;
                btnDelCards.FlatAppearance.BorderSize = 0;
                btnDelCards.Width = 55;
                btnDelCards.Height = 20;
                btnDelCards.Location = new Point(xCard + 89, yCard + 35 - 2);
                btnDelCards.FlatStyle = FlatStyle.Flat;
                btnDelCards.BringToFront();
                btnDelCards.BackColor = Color.Turquoise;

                //кнопка добавления задачи
                Button btnAddTasks = new Button();
                btnAddTasks.Text = "+ task";
                btnAddTasks.Font = new Font("OCR-A BT", 10, btnRedactCards.Font.Style);
                btnAddTasks.ForeColor = Color.MidnightBlue;
                btnAddTasks.FlatAppearance.BorderSize = 0;
                btnAddTasks.Width = buttonWidthCard - 12;
                btnAddTasks.Height = 30 - 4;
                btnAddTasks.Location = new Point(xCard + 6, yCard + 75 + 30 + 4);
                btnAddTasks.FlatStyle = FlatStyle.Flat;
                btnAddTasks.BringToFront();
                btnAddTasks.BackColor = Color.Turquoise;

                //описание карточки
                Label lblDescribe = new Label();
                lblDescribe.Text = card.info;
                lblDescribe.Width = buttonWidthCard - 12;
                lblDescribe.Height = buttonHeightCard + 20;
                lblDescribe.Location = new Point(xCard + 6, yCard + 56);
                lblDescribe.BackColor = Color.DarkTurquoise;
                lblDescribe.Font = new Font("OCR-A BT", 10, labelDeskName.Font.Style);
                lblDescribe.ForeColor = Color.White;
                lblDescribe.BringToFront();

                //участники карточки
                Label lblParticipants = new Label();
                lblParticipants.Text = "participants: ";
                lblParticipants.Width = lblDescribe.Width;
                lblParticipants.Height = 30;
                lblParticipants.Location = new Point(lblDescribe.Location.X, lblDescribe.Location.Y + lblDescribe.Height);
                lblParticipants.BackColor = Color.DarkTurquoise;
                lblParticipants.Font = new Font("OCR-A BT", 8, lblParticipants.Font.Style);
                lblParticipants.ForeColor = Color.White;
                lblParticipants.BringToFront();

                //задняя панель карточки
                PictureBox pictureBox = new PictureBox();
                pictureBox.BackColor = Color.White;
                pictureBox.BorderStyle = BorderStyle.None;
                pictureBox.Width = buttonWidthCard;
                pictureBox.Height = 80;
                pictureBox.Location = new Point(xCard, yCard);

                //вся задняя панель (с чекбоксами)
                PictureBox bckgCard = new PictureBox();
                bckgCard.BackColor = Color.White;
                bckgCard.BorderStyle = BorderStyle.None;
                bckgCard.Width = buttonWidthCard;
                bckgCard.Height = (card.checkList.tasks.Count * 33) + 30 + 30;
                bckgCard.Location = new Point(xCard, yCard + 80);
                bckgCard.BringToFront();

                buttonsCards.Add(button);
                buttonsRedactCards.Add(btnRedactCards);
                buttonsAddTask.Add(btnAddTasks);
                buttonsDelCards.Add(btnDelCards);
                lblDescriptionCards.Add(lblDescribe);
                lblParticipantsCards.Add(lblParticipants);
                picturesUnderCards.Add(pictureBox);
                backgroundCards.Add(bckgCard);

                xCard += buttonWidthCard + spacingCard;

                Card selectedCard = cards[buttonsCards.IndexOf(button)];
                Check check = db.TasksReturnCard(selectedCard);
                List<Task> tasks = db.TasksReturnCheck(check);
                List<CheckBox> checkBoxesTasks = new();

                int taskWidthTask = 140;
                int taskHeightTask = 30;
                int spacingTask = 3;
                int xTask = xCard - buttonWidthCard - 7;
                int yTask = yCard - buttonHeightCard + 140 + 30;

                //добавление новой задачи в чеклист
                btnAddTasks.Click += (sender, e) =>
                {
                    string taskName = Microsoft.VisualBasic.Interaction.InputBox("Введите название задачи:", "Добавление задачи", "");
                    if (taskName != "")
                    {
                        string name = taskName;
                        string card_id = card.id;
                        Task newTask = new Task(name, card_id);
                        //db.AddTask(newTask);
                        db.CreateTask(name, card_id);

                        CheckBox checkBox = new CheckBox();
                        checkBox.Text = newTask.name;
                        checkBox.Width = taskWidthTask;
                        checkBox.Height = taskHeightTask;
                        checkBox.Location = new Point(xTask, yTask);
                        checkBox.Font = new Font("OCR-A BT", 10, labelDeskName.Font.Style);
                        checkBox.BackColor = Color.White;


                        yTask += taskHeightTask + spacingTask;
                        bckgCard.Height += 30 + 3;
                        checkBoxesTasks.Add(checkBox);
                        this.Controls.Add(checkBox);
                        this.Controls.Add(bckgCard);

                        checkBox.CheckedChanged += (sender, e) =>
                        {
                            if (checkBox.Checked)
                            {
                                db.TaskBoolChangerToTrue(newTask);
                                checkBox.ForeColor = Color.DarkTurquoise;
                                newTask.done = true;
                            }
                            else
                            {
                                db.TaskBoolChangerToFalse(newTask);
                                checkBox.ForeColor = Color.MidnightBlue;
                                newTask.done = false;
                            }
                        };
                    }
                };

                foreach (Task task in tasks)
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.Text = task.name;
                    checkBox.Width = taskWidthTask;
                    checkBox.Height = taskHeightTask;
                    checkBox.Location = new Point(xTask, yTask);
                    checkBox.Font = new Font("OCR-A BT", 10, labelDeskName.Font.Style);
                    checkBox.BackColor = Color.White;
                    this.Controls.Add(checkBox);
                    yTask += taskHeightTask + spacingTask;

                    checkBoxesTasks.Add(checkBox);

                    btnDelCards.Click += new EventHandler(btnDelCardsCheckBox_Click);
                    void btnDelCardsCheckBox_Click(object sender, EventArgs e)
                    {
                        this.Controls.Remove(checkBox);
                    }
                }
                foreach (CheckBox checkBox in checkBoxesTasks)
                {
                    Task task = tasks[checkBoxesTasks.IndexOf(checkBox)];

                    if (task.done == true)
                    {
                        checkBox.Checked = true;
                        checkBox.ForeColor = Color.DarkTurquoise;
                    }
                    if (task.done == false)
                    {
                        checkBox.ForeColor = Color.MidnightBlue;
                    }

                    checkBox.CheckedChanged += (sender, e) =>
                    {
                        if (checkBox.Checked)
                        {
                            db.TaskBoolChangerToTrue(task);
                            checkBox.ForeColor = Color.DarkTurquoise;
                            task.done = true;
                        }
                        else
                        {
                            db.TaskBoolChangerToFalse(task);
                            checkBox.ForeColor = Color.MidnightBlue;
                            task.done = false;
                        }
                    };
                }

                //кнопка добавление карточки
                Button createCardButton = new Button();
                createCardButton.Text = "+ card";
                createCardButton.Location = new Point(400, 40);
                createCardButton.BackColor = Color.Orange;
                createCardButton.Font = new Font("OCR-A BT", 16, createCardButton.Font.Style);
                createCardButton.ForeColor = Color.MidnightBlue;
                createCardButton.FlatStyle = FlatStyle.Flat;
                createCardButton.FlatAppearance.BorderSize = 0;
                createCardButton.Width = 100;
                createCardButton.Height = 50;
                createCardButton.BringToFront();
                createCardButton.Click += new EventHandler(CreateCardButton_Click);
                this.Controls.Add(createCardButton);
                void CreateCardButton_Click(object sender, EventArgs e)
                {
                    string cardName = Microsoft.VisualBasic.Interaction.InputBox("Введите название карточки:", "Создание карточки", "");
                    string cardInfo = Microsoft.VisualBasic.Interaction.InputBox("Введите описание карточки:", "Создание карточки", "");
                    if (cardName != "")
                    {
                        Card newCard = new Card(u1.id, cardName, false, cardInfo, db);
                        cards.Add(newCard);
                        bool checkNewCard = db.AddCard(newCard);

                        // Создание элементов управления для новой карточки
                        Button button = new Button();
                        button.Text = newCard.name;
                        button.Location = new Point(xCard + 100, yCard);
                        Button btnRedactCards = new Button();
                        btnRedactCards.Text = "rename";
                        btnRedactCards.Location = new Point(xCard + 6, yCard + 35 - 2);
                        btnRedactCards.Click += new EventHandler(btnRedactCards_Click);
                        PictureBox bckgCard = new PictureBox();
                        bckgCard.BackColor = Color.White;
                        bckgCard.Location = new Point(xCard, yCard + 80);

                        buttonsCards.Add(button);
                        buttonsRedactCards.Add(btnRedactCards);
                        lblDescriptionCards.Add(lblDescribe);
                        picturesUnderCards.Add(pictureBox);
                        backgroundCards.Add(bckgCard);

                        xCard += buttonWidthCard + spacingCard;

                        // Добавление задач к карточке
                        Card selectedCard = cards[buttonsCards.IndexOf(button)];
                        Check check = db.TasksReturnCard(selectedCard);
                        List<Task> tasks = db.TasksReturnCheck(check);
                        List<CheckBox> checkBoxesTasks = new List<CheckBox>();
                        foreach (Task task in tasks)
                        {
                            CheckBox checkBox = new CheckBox();
                            checkBox.Text = task.name;
                            checkBox.Location = new Point(xTask, yTask);
                            this.Controls.Add(checkBox);
                            yTask += taskHeightTask + spacingTask;

                            checkBoxesTasks.Add(checkBox);

                            btnDelCards.Click += new EventHandler(btnDelCardsCheckBox_Click);
                            void btnDelCardsCheckBox_Click(object sender, EventArgs e)
                            {
                                this.Controls.Remove(checkBox);
                            }
                        }
                        foreach (CheckBox checkBox in checkBoxesTasks)
                        {
                            Task task = tasks[checkBoxesTasks.IndexOf(checkBox)];

                            if (task.done == true)
                            {
                                checkBox.Checked = true;
                                checkBox.ForeColor = Color.DarkTurquoise;
                            }
                            if (task.done == false)
                            {
                                checkBox.ForeColor = Color.MidnightBlue;
                            }

                            checkBox.CheckedChanged += (sender, e) =>
                            {
                                if (checkBox.Checked)
                                {
                                    db.TaskBoolChangerToTrue(task);
                                    checkBox.ForeColor = Color.DarkTurquoise;
                                    task.done = true;
                                }
                                else
                                {
                                    db.TaskBoolChangerToFalse(task);
                                    checkBox.ForeColor = Color.MidnightBlue;
                                    task.done = false;
                                }
                            };
                        }
                        Application.Restart();
                    }
                }

                this.Controls.Add(labelParticipants);
                this.Controls.Add(labelDeskName);

                this.Controls.Add(button);
                this.Controls.Add(btnRedactCards);
                this.Controls.Add(btnDelCards);
                this.Controls.Add(btnAddTasks);
                this.Controls.Add(lblDescribe);
                this.Controls.Add(lblParticipants);
                this.Controls.Add(pictureBox);
                this.Controls.Add(bckgCard);


                btnDelCards.Click += new EventHandler(btnDelCards_Click);
                void btnDelCards_Click(object sender, EventArgs e)
                {
                    this.Controls.Remove(button);
                    this.Controls.Remove(btnRedactCards);
                    this.Controls.Remove(btnAddTasks);
                    this.Controls.Remove(btnDelCards);
                    this.Controls.Remove(lblDescribe);
                    this.Controls.Remove(pictureBox);
                    this.Controls.Remove(bckgCard);
                    db.DelCard(card);
                    Application.Restart();
                }
            }
            this.Update();
        }
        /*            foreach (Button button in buttonsCards)
                        {
                            button.Click += (sender, e) =>
                            {
                                // Очистка списка задач на форме
                                this.Controls.OfType<CheckBox>().ToList().ForEach(c => c.Dispose());

                                // Получение выбранной карточки и списка задач для этой карточки из базы данных
                                Card selectedCard = cards[buttonsCards.IndexOf(button)];
                                Check check = db.TasksReturnCard(selectedCard);
                                List<Task> tasks = db.TasksReturnCheck(check);
                                List<CheckBox> checkBoxesTasks = new();

                                // Отображение списка задач в виде checkBox
                                int taskWidthTask = 150;
                                int taskHeightTask = 30;
                                int spacingTask = 3;
                                int xTask = xCard;
                                int yTask = yCard - buttonHeightCard + 135;
                                foreach (Task task in tasks)
                                {
                                    CheckBox checkBox = new CheckBox();
                                    checkBox.Text = task.name;
                                    checkBox.Width = taskWidthTask;
                                    checkBox.Height = taskHeightTask;
                                    checkBox.Location = new Point(xTask, yTask);
                                    checkBox.Font = new Font("OCR-A BT", 10, labelDeskName.Font.Style);
                                    this.Controls.Add(checkBox);
                                    yTask += taskHeightTask + spacingTask;

                                    checkBoxesTasks.Add(checkBox);
                                }
                                foreach (CheckBox checkBox in checkBoxesTasks)
                                {
                                    Task task = tasks[checkBoxesTasks.IndexOf(checkBox)];

                                    if (task.done == true)
                                    {
                                        checkBox.Checked = true;
                                        checkBox.ForeColor = Color.DarkTurquoise;
                                    }
                                    if (task.done == false)
                                    {
                                        checkBox.ForeColor = Color.MidnightBlue;
                                    }

                                    checkBox.CheckedChanged += (sender, e) =>
                                    {
                                        if (checkBox.Checked)
                                        {
                                            db.TaskBoolChangerToTrue(task);
                                            checkBox.ForeColor = Color.DarkTurquoise;
                                            task.done = true;
                                        }
                                        else
                                        {
                                            db.TaskBoolChangerToFalse(task);
                                            checkBox.ForeColor = Color.MidnightBlue;
                                            task.done = false;
                                        }
                                    };
                                }
                            };
                        }*/
        /*
                // Обработчик события Click для кнопки переименования элемента
                private void btnRedactCards_Click(object sender, EventArgs e)
                {
                    // Получаем новое название элемента от пользователя
                    string newName = Microsoft.VisualBasic.Interaction.InputBox("Введите новое название карточки:", "Переименование карточки", "");
                    // Проверяем, что пользователь ввел название элемента
                    if (!string.IsNullOrEmpty(newName))
                    {
                        // Получаем кнопку, которую нужно переименовать
                        Button button = (Button)sender;
                        // Присваиваем новое название элементу
                        button.Text = newName;
                    }
                }*/

        private void LoadDesk(Desk u1)
        {
            db.ShowCards(u1);
            List<Card> cards = db.CardsReturn(u1);
            //отображение автора
            Label labelParticipants = new Label();
            labelParticipants.Text = "creator: " + u1.owner.name;
            labelParticipants.Width = 200;
            labelParticipants.Height = 50;
            labelParticipants.Location = new Point(700, 40);
            labelParticipants.BackColor = Color.Orange;
            labelParticipants.Font = new Font("OCR-A BT", 16, labelParticipants.Font.Style);
            labelParticipants.ForeColor = Color.MidnightBlue;
            labelParticipants.BringToFront();

            //показ названия доски
            Label labelDeskName = new Label();
            labelDeskName.Text = u1.name;
            labelDeskName.Width = 900;
            labelDeskName.Height = 50;
            labelDeskName.Location = new Point(10, 40);
            labelDeskName.BackColor = Color.Orange;
            labelDeskName.Font = new Font("OCR-A BT", 18, labelDeskName.Font.Style);
            labelDeskName.ForeColor = Color.MidnightBlue;
            labelDeskName.BringToFront();

            List<Button> buttonsCards = new();
            List<PictureBox> picturesUnderCards = new();
            List<Button> buttonsRedactCards = new();
            List<Button> buttonsAddTask = new();
            List<Button> buttonsDelCards = new();
            List<Label> lblParticipantsCards = new();
            List<Label> lblDescriptionCards = new();
            List<PictureBox> backgroundCards = new();

            int buttonWidthCard = 150;
            int buttonHeightCard = 30;
            int spacingCard = 10;
            int xCard = 10;
            int yCard = 100;

            foreach (Card card in cards)
            {
                db.CountParticipants(card);

                //кнопка с названием карточки
                Button button = new Button();
                button.Text = card.name;
                button.Font = new Font("OCR-A BT", 9, button.Font.Style);
                button.ForeColor = Color.MidnightBlue;
                button.Width = buttonWidthCard;
                button.Height = buttonHeightCard;
                button.Location = new Point(xCard, yCard);
                button.FlatStyle = FlatStyle.Flat;
                button.BringToFront();
                button.BackColor = Color.Orange;
                button.FlatAppearance.BorderSize = 0;

                //кнопка редактирования карточки
                Button btnRedactCards = new Button();
                btnRedactCards.Text = "rename";
                btnRedactCards.Font = new Font("OCR-A BT", 8, btnRedactCards.Font.Style);
                btnRedactCards.ForeColor = Color.MidnightBlue;
                btnRedactCards.FlatAppearance.BorderSize = 0;
                btnRedactCards.Width = 55;
                btnRedactCards.Height = 20;
                btnRedactCards.Location = new Point(xCard + 6, yCard + 35 - 2);
                btnRedactCards.FlatStyle = FlatStyle.Flat;
                btnRedactCards.BringToFront();
                btnRedactCards.BackColor = Color.Turquoise;

                btnRedactCards.Click += new EventHandler(btnRedactCards_Click);
                void btnRedactCards_Click(object sender, EventArgs e)
                {
                    string newButtonText = Microsoft.VisualBasic.Interaction.InputBox("Введите новое название кнопки:", "Переименование кнопки", button.Text);
                    db.ChangeNameCard(card, newButtonText);
                    button.Text = newButtonText;
                }

                //кнопка удаления карточки
                Button btnDelCards = new Button();
                btnDelCards.Text = "delete";
                btnDelCards.Font = new Font("OCR-A BT", 8, btnRedactCards.Font.Style);
                btnDelCards.ForeColor = Color.MidnightBlue;
                btnDelCards.FlatAppearance.BorderSize = 0;
                btnDelCards.Width = 55;
                btnDelCards.Height = 20;
                btnDelCards.Location = new Point(xCard + 89, yCard + 35 - 2);
                btnDelCards.FlatStyle = FlatStyle.Flat;
                btnDelCards.BringToFront();
                btnDelCards.BackColor = Color.Turquoise;

                //кнопка добавления задачи
                Button btnAddTasks = new Button();
                btnAddTasks.Text = "+ task";
                btnAddTasks.Font = new Font("OCR-A BT", 10, btnRedactCards.Font.Style);
                btnAddTasks.ForeColor = Color.MidnightBlue;
                btnAddTasks.FlatAppearance.BorderSize = 0;
                btnAddTasks.Width = buttonWidthCard - 12;
                btnAddTasks.Height = 30 - 4;
                btnAddTasks.Location = new Point(xCard + 6, yCard + 75 + 30 + 4);
                btnAddTasks.FlatStyle = FlatStyle.Flat;
                btnAddTasks.BringToFront();
                btnAddTasks.BackColor = Color.Turquoise;

                //описание карточки
                Label lblDescribe = new Label();
                lblDescribe.Text = card.info;
                lblDescribe.Width = buttonWidthCard - 12;
                lblDescribe.Height = buttonHeightCard + 20;
                lblDescribe.Location = new Point(xCard + 6, yCard + 56);
                lblDescribe.BackColor = Color.DarkTurquoise;
                lblDescribe.Font = new Font("OCR-A BT", 10, labelDeskName.Font.Style);
                lblDescribe.ForeColor = Color.White;
                lblDescribe.BringToFront();

                //участники карточки
                Label lblParticipants = new Label();
                lblParticipants.Text = "participants: ";
                lblParticipants.Width = lblDescribe.Width;
                lblParticipants.Height = 30;
                lblParticipants.Location = new Point(lblDescribe.Location.X, lblDescribe.Location.Y + lblDescribe.Height);
                lblParticipants.BackColor = Color.DarkTurquoise;
                lblParticipants.Font = new Font("OCR-A BT", 8, lblParticipants.Font.Style);
                lblParticipants.ForeColor = Color.White;
                lblParticipants.BringToFront();

                //задняя панель карточки
                PictureBox pictureBox = new PictureBox();
                pictureBox.BackColor = Color.White;
                pictureBox.BorderStyle = BorderStyle.None;
                pictureBox.Width = buttonWidthCard;
                pictureBox.Height = 80;
                pictureBox.Location = new Point(xCard, yCard);

                //вся задняя панель (с чекбоксами)
                PictureBox bckgCard = new PictureBox();
                bckgCard.BackColor = Color.White;
                bckgCard.BorderStyle = BorderStyle.None;
                bckgCard.Width = buttonWidthCard;
                bckgCard.Height = (card.checkList.tasks.Count * 33) + 30 + 30;
                bckgCard.Location = new Point(xCard, yCard + 80);
                bckgCard.BringToFront();

                buttonsCards.Add(button);
                buttonsRedactCards.Add(btnRedactCards);
                buttonsAddTask.Add(btnAddTasks);
                buttonsDelCards.Add(btnDelCards);
                lblDescriptionCards.Add(lblDescribe);
                lblParticipantsCards.Add(lblParticipants);
                picturesUnderCards.Add(pictureBox);
                backgroundCards.Add(bckgCard);

                xCard += buttonWidthCard + spacingCard;

                Card selectedCard = cards[buttonsCards.IndexOf(button)];
                Check check = db.TasksReturnCard(selectedCard);
                List<Task> tasks = db.TasksReturnCheck(check);
                List<CheckBox> checkBoxesTasks = new();

                int taskWidthTask = 140;
                int taskHeightTask = 30;
                int spacingTask = 3;
                int xTask = xCard - buttonWidthCard - 7;
                int yTask = yCard - buttonHeightCard + 140 + 30;

                //добавление новой задачи в чеклист
                btnAddTasks.Click += (sender, e) =>
                {
                    string taskName = Microsoft.VisualBasic.Interaction.InputBox("Введите название задачи:", "Добавление задачи", "");
                    if (taskName != "")
                    {
                        string name = taskName;
                        string card_id = card.id;
                        Task newTask = new Task(name, card_id);
                        //db.AddTask(newTask);
                        db.CreateTask(name, card_id);

                        CheckBox checkBox = new CheckBox();
                        checkBox.Text = newTask.name;
                        checkBox.Width = taskWidthTask;
                        checkBox.Height = taskHeightTask;
                        checkBox.Location = new Point(xTask, yTask);
                        checkBox.Font = new Font("OCR-A BT", 10, labelDeskName.Font.Style);
                        checkBox.BackColor = Color.White;


                        yTask += taskHeightTask + spacingTask;
                        bckgCard.Height += 30 + 3;
                        checkBoxesTasks.Add(checkBox);
                        this.Controls.Add(checkBox);
                        this.Controls.Add(bckgCard);

                        checkBox.CheckedChanged += (sender, e) =>
                        {
                            if (checkBox.Checked)
                            {
                                db.TaskBoolChangerToTrue(newTask);
                                checkBox.ForeColor = Color.DarkTurquoise;
                                newTask.done = true;
                            }
                            else
                            {
                                db.TaskBoolChangerToFalse(newTask);
                                checkBox.ForeColor = Color.MidnightBlue;
                                newTask.done = false;
                            }
                        };
                    }
                };

                foreach (Task task in tasks)
                {
                    CheckBox checkBox = new CheckBox();
                    checkBox.Text = task.name;
                    checkBox.Width = taskWidthTask;
                    checkBox.Height = taskHeightTask;
                    checkBox.Location = new Point(xTask, yTask);
                    checkBox.Font = new Font("OCR-A BT", 10, labelDeskName.Font.Style);
                    checkBox.BackColor = Color.White;
                    this.Controls.Add(checkBox);
                    yTask += taskHeightTask + spacingTask;

                    checkBoxesTasks.Add(checkBox);

                    btnDelCards.Click += new EventHandler(btnDelCardsCheckBox_Click);
                    void btnDelCardsCheckBox_Click(object sender, EventArgs e)
                    {
                        this.Controls.Remove(checkBox);
                    }
                }
                foreach (CheckBox checkBox in checkBoxesTasks)
                {
                    Task task = tasks[checkBoxesTasks.IndexOf(checkBox)];

                    if (task.done == true)
                    {
                        checkBox.Checked = true;
                        checkBox.ForeColor = Color.DarkTurquoise;
                    }
                    if (task.done == false)
                    {
                        checkBox.ForeColor = Color.MidnightBlue;
                    }

                    checkBox.CheckedChanged += (sender, e) =>
                    {
                        if (checkBox.Checked)
                        {
                            db.TaskBoolChangerToTrue(task);
                            checkBox.ForeColor = Color.DarkTurquoise;
                            task.done = true;
                        }
                        else
                        {
                            db.TaskBoolChangerToFalse(task);
                            checkBox.ForeColor = Color.MidnightBlue;
                            task.done = false;
                        }
                    };
                }

                //кнопка добавление карточки
                Button createCardButton = new Button();
                createCardButton.Text = "+ card";
                createCardButton.Location = new Point(400, 40);
                createCardButton.BackColor = Color.Orange;
                createCardButton.Font = new Font("OCR-A BT", 16, createCardButton.Font.Style);
                createCardButton.ForeColor = Color.MidnightBlue;
                createCardButton.FlatStyle = FlatStyle.Flat;
                createCardButton.FlatAppearance.BorderSize = 0;
                createCardButton.Width = 100;
                createCardButton.Height = 50;
                createCardButton.BringToFront();
                createCardButton.Click += new EventHandler(CreateCardButton_Click);
                this.Controls.Add(createCardButton);
                void CreateCardButton_Click(object sender, EventArgs e)
                {
                    string cardName = Microsoft.VisualBasic.Interaction.InputBox("Введите название карточки:", "Создание карточки", "");
                    string cardInfo = Microsoft.VisualBasic.Interaction.InputBox("Введите описание карточки:", "Создание карточки", "");
                    if (cardName != "")
                    {
                        Card newCard = new Card(u1.id, cardName, false, cardInfo, db);
                        cards.Add(newCard);
                        bool checkNewCard = db.AddCard(newCard);

                        // Создание элементов управления для новой карточки
                        Button button = new Button();
                        button.Text = newCard.name;
                        button.Location = new Point(xCard + 100, yCard);
                        Button btnRedactCards = new Button();
                        btnRedactCards.Text = "rename";
                        btnRedactCards.Location = new Point(xCard + 6, yCard + 35 - 2);
                        btnRedactCards.Click += new EventHandler(btnRedactCards_Click);
                        PictureBox bckgCard = new PictureBox();
                        bckgCard.BackColor = Color.White;
                        bckgCard.Location = new Point(xCard, yCard + 80);

                        buttonsCards.Add(button);
                        buttonsRedactCards.Add(btnRedactCards);
                        lblDescriptionCards.Add(lblDescribe);
                        picturesUnderCards.Add(pictureBox);
                        backgroundCards.Add(bckgCard);

                        xCard += buttonWidthCard + spacingCard;

                        // Добавление задач к карточке
                        Card selectedCard = cards[buttonsCards.IndexOf(button)];
                        Check check = db.TasksReturnCard(selectedCard);
                        List<Task> tasks = db.TasksReturnCheck(check);
                        List<CheckBox> checkBoxesTasks = new List<CheckBox>();
                        foreach (Task task in tasks)
                        {
                            CheckBox checkBox = new CheckBox();
                            checkBox.Text = task.name;
                            checkBox.Location = new Point(xTask, yTask);
                            this.Controls.Add(checkBox);
                            yTask += taskHeightTask + spacingTask;

                            checkBoxesTasks.Add(checkBox);

                            btnDelCards.Click += new EventHandler(btnDelCardsCheckBox_Click);
                            void btnDelCardsCheckBox_Click(object sender, EventArgs e)
                            {
                                this.Controls.Remove(checkBox);
                            }
                        }
                        foreach (CheckBox checkBox in checkBoxesTasks)
                        {
                            Task task = tasks[checkBoxesTasks.IndexOf(checkBox)];

                            if (task.done == true)
                            {
                                checkBox.Checked = true;
                                checkBox.ForeColor = Color.DarkTurquoise;
                            }
                            if (task.done == false)
                            {
                                checkBox.ForeColor = Color.MidnightBlue;
                            }

                            checkBox.CheckedChanged += (sender, e) =>
                            {
                                if (checkBox.Checked)
                                {
                                    db.TaskBoolChangerToTrue(task);
                                    checkBox.ForeColor = Color.DarkTurquoise;
                                    task.done = true;
                                }
                                else
                                {
                                    db.TaskBoolChangerToFalse(task);
                                    checkBox.ForeColor = Color.MidnightBlue;
                                    task.done = false;
                                }
                            };
                        }
                        Application.Restart();
                    }
                }

                this.Controls.Add(labelParticipants);
                this.Controls.Add(labelDeskName);

                this.Controls.Add(button);
                this.Controls.Add(btnRedactCards);
                this.Controls.Add(btnDelCards);
                this.Controls.Add(btnAddTasks);
                this.Controls.Add(lblDescribe);
                this.Controls.Add(lblParticipants);
                this.Controls.Add(pictureBox);
                this.Controls.Add(bckgCard);


                btnDelCards.Click += new EventHandler(btnDelCards_Click);
                void btnDelCards_Click(object sender, EventArgs e)
                {
                    this.Controls.Remove(button);
                    this.Controls.Remove(btnRedactCards);
                    this.Controls.Remove(btnAddTasks);
                    this.Controls.Remove(btnDelCards);
                    this.Controls.Remove(lblDescribe);
                    this.Controls.Remove(pictureBox);
                    this.Controls.Remove(bckgCard);
                    db.DelCard(card);
                    Application.Restart();
                }
            }
            this.Update();
        }

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

    }
}
