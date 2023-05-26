using OfficeOpenXml;
using TaskManager;

namespace InfoBase
{
    internal class DataBase
    {
        public List<Desk> desks;//доски с заданиями 
        public List<User> users;//пользователи 

        public List<string> user_ids;//все идентификаторы пользователей 
        public List<string> desk_ids;//все идентификаторы досок 
        public List<string> card_ids;//все идентификаторы карточек 

        public string logfile_path; //путь к папке с логами 
        public string data_path; //путь к таблице с данными 
        public string users_path; //путь к таблице с юзерами 
        public string cards_path; //путь к папке с карточками 
        private readonly bool consoleLogging;//делать логи в консоли или нет (выключить, если нужно будет работать с визуализацией) 

        public string version { get; }//версия программы 
        public string progname { get; }//имя программы 
        private int log_counter; //для LogState
        ////////////////////Переменные, необходимые для работы всей датабазы////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////

        //некоторые вспомогательные инструменты
        public static DateTime Date(string date)//для дат формата "dd.MM.yyyy hh:mm" (при первой неудаче форматирует как "yyyy.MM.dd hh:mm")
        {
            string[] datenums = date.Split(' ')[0].Split('.');
            string[] timenums = date.Split(' ')[1].Split(':');
            string error = String.Empty;
            DateTime data;
            try
            {
                data = new DateTime(int.Parse(datenums[0]), int.Parse(datenums[1]), int.Parse(datenums[2]),
                                int.Parse(timenums[0]), int.Parse(timenums[1]), 0);
                return data;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                data = new DateTime(int.Parse(datenums[2]), int.Parse(datenums[1]), int.Parse(datenums[0]),
                                int.Parse(timenums[0]), int.Parse(timenums[1]), 0);
                return data;
            }
        }
        public string? CreateDayList(string card_id)//создание макета списка дня 
        {
            //создаем новый документ 
            string fullPath = cards_path + card_id + ".desk";

            if (!File.Exists(fullPath)) { File.Create(fullPath); return null; }
            else
            {
                return fullPath;
            }
        }
        public void LogState(string message)//логирование 
        {
            log_counter++;
            bool newFile = false;
            string dirWithLogName = logfile_path + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            if (!File.Exists(dirWithLogName))
            {
                using (File.Create(dirWithLogName)) { }
                newFile = true;
            };

            using StreamWriter writer = new(dirWithLogName, true);
            int n = 20;
            string line = string.Empty;
            for (int i = 0; i < n; i++) { line += '-'; }
            if (log_counter == 1)
            {
                if (newFile)
                {
                    writer.Write(line + " НОВЫЙ ЗАПУСК " + DateTime.Now.ToString("HH:mm:ss.fff") + " " + line + '\n');
                }
                else
                {
                    writer.Write('\n' + line + " НОВЫЙ ЗАПУСК " + DateTime.Now.ToString("HH:mm:ss.fff") + " " + line + '\n');
                }

                if (consoleLogging)
                {
                    Console.WriteLine('\n' + line + " ЛОГ ЗАПУСКА " + DateTime.Now.ToString("HH:mm:ss.fff") + " " + line + '\n');
                }
            }
            writer.Write(DateTime.Now.ToString("HH:mm:ss.fff") + " : " + message + '\n');
            if (consoleLogging)
            {
                if (message.Contains('\n'))
                {
                    Console.Write(DateTime.Now.ToString("HH:mm:ss.fff") + " : " + message);
                }
                else
                {
                    Console.Write(DateTime.Now.ToString("HH:mm:ss.fff") + " : " + message + '\n');
                }
            }
        }
        public User RandLogPass(string name)//создание автоматически сгенерированного пользователя
        {
            // Создание генератора случайных чисел
            Random random = new Random();

            // Создание случайного логина
            string login = "user" + user_ids.Count+1;

            // Создание случайного пароля
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string password = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());

            LogState($"Успешно зарезервирован в памяти пользователь: {login}|{password}");
            return new(login, password, name, this);
        }

        //рабочие инструменты базы данных
        public User? GetUser(string id)//найти пользователя по идентификатору 
        {
            bool cond = false;
            foreach (User user in users)
            {
                if (user.id == id)
                {
                    cond = true;
                    return user;
                }
            }
            if (!cond)
            {
                LogState($"Пользователя с идентификатором {id} нету в базе данных");
            }
            return null;
        }
        public User? GetFullUser(string login, string name)//найти пользователя по логину и имени 
        {
            bool cond = false;
            foreach (User user in users)
            {
                if (user.name == name && user.login == login) { cond = true; return user; }
            }
            if (!cond)
            {
                LogState($"Пользователя с именем {name} и логином {login} нету в базе данных");
            }

            return null;
        }
        public Card? GetCard(string id)//найти карточку по идентификатору 
        {
            foreach (var desk in desks)
            {
                foreach (var card in desk.cards)
                {
                    if (card.id == id)
                    {
                        return card;
                    }
                }
            }
            LogState($"Не найдено задания с идентификатором {id}");
            return null;
        }
        public Desk? GetDesk(string id)//найти доску по идентификатору 
        {
            foreach (Desk desk in desks)
            {
                if (desk.id == id)
                {
                    return desk;
                }
            }
            LogState($"Аудитория с номером {id} не найдена");
            return null;
        }

        public bool MoveCard()///смещение позиции карточки на доске 
        { return true; }
        public bool MoveDesk()///смещение позиции доски в списке всех досок 
        { return true; }
        public bool MoveUser()///смещение позиции пользователя в списке всех пользователей 
        { return true; }
        public bool MoveTask()///смещение позиции пункта в чек-листе 
        { return true; }

        public bool SetCard(Card old_card, Card new_card)///сменить одну карточку на другую 
        {
            Desk desk = GetDesk(old_card.desk.id);
            if (desk.timetable.Remove(old_card))
            {
                _ = desk.AddCard(new_card, this);
                string filePath;

                try
                {
                    filePath = cards_path + old_card.startTime.ToString("yyyy.MM.dd") + ".desk"; // путь к файлу
                }
                catch (Exception ex) { LogState($"Возникла следующая ошибка: {ex}"); return false; }

                /*Название предмета 1 | 9:00 | 10:00 | Преподаватель 1 | Доп описание для Название предмета 1 1 | a1*/
                string searchLine = old_card.name + '|' + old_card.startTime.ToString("H:mm") + '|' + old_card.endTime.ToString("H:mm") + '|'
                                    + old_card.teacher.name + '|' + old_card.subname + '|' + old_card.desk.id;// строка, которую нужно заменить
                string newLine = new_card.name + '|' + new_card.startTime.ToString("H:mm") + '|' + new_card.endTime.ToString("H:mm") + '|'
                                 + new_card.teacher.name + '|' + new_card.subname + '|' + new_card.desk.id; // новая строка, которой заменится найденная строка

                // Открываем файл для чтения и записи
                try
                {
                    using StreamReader reader = new(filePath);
                    // Создаем временный файл для записи
                    string tempFilePath = System.IO.Path.GetTempFileName();

                    // Открываем временный файл для записи
                    using (StreamWriter writer = new(tempFilePath))
                    {
                        string line;
                        string? temp_line = string.Empty;
                        bool lineFound = false;
                        bool sucess = false;

                        // Читаем файл построчно
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (lineFound)
                            {
                                if (temp_line != string.Empty)
                                {
                                    writer.WriteLine(temp_line);
                                }

                                lineFound = false;
                                sucess = true;
                            }

                            if (line.Contains(searchLine))
                            {
                                writer.WriteLine(newLine);
                                lineFound = true;
                                foreach (User user in new_card.participators)
                                {
                                    writer.WriteLine($"{user.login}|{user.name}");
                                }
                                while ((line = reader.ReadLine()) != null)
                                {
                                    if (line.Split('|').Length > 2)
                                    {
                                        temp_line = line;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                writer.WriteLine(line);
                            }
                        }

                        if (lineFound)
                        {
                            if (temp_line != string.Empty)
                            {
                                writer.WriteLine(temp_line);
                            }

                            lineFound = false;

                        }

                        // Если строка не была найдена
                        if (!sucess)
                        {
                            LogState($"Строка для замены \"{searchLine}\" не найдена");
                            return false;
                        }
                    }

                    // Закрываем файлы
                    reader.Close();

                    // Заменяем исходный файл временным файлом
                    File.Delete(filePath);
                    File.Move(tempFilePath, filePath);
                }
                catch (IOException ex)
                {
                    LogState("Возникла следующая ошибка: " + ex.Message);
                    return false;
                }

                return true;
            }
            else
            {
                LogState("Не получилось изменить запись (возможно, заменяемой вами записи не существует)");
                return false;
            }
        }
        public bool SetUser(User old_user, User new_user)///сменить одного пользователя на другого 
        {
            string fullPath = users_path;
            ExcelPackage excel = new(new FileInfo(fullPath));
            ExcelWorksheet? users = excel.Workbook.Worksheets["Данные"];

            if (users == null)
            {
                LogState("Пересмотри данные пользователей");
                return false;
            }

            int indexToIns = -1;
            if (this.users.FindIndex(x => x == old_user) != -1)
            {
                indexToIns = this.users.FindIndex(x => x == old_user);
            }
            else
            {
                LogState("Не получилось изменить запись (возможно, заменяемого вами пользователя не существует)");
                return false;
            }

            if (this.users.Remove(old_user))
            {
                this.users.Insert(indexToIns, new_user);
                int index = 1;
                bool cond = true;
                while (index <= users.Dimension.End.Row)
                {
                    string? user_login = users.Cells[$"A{index}"].Value?.ToString();
                    string? user_password = users.Cells[$"B{index}"].Value?.ToString();
                    string? user_access = users.Cells[$"C{index}"].Value?.ToString();
                    string? user_name = users.Cells[$"D{index}"].Value?.ToString();
                    if (user_password == null || user_login == null || user_access == null || user_name == null)
                    {
                        if (user_password == null && user_login == null && user_access == null && user_name == null)
                        {

                        }
                        else
                        {
                            LogState($"Строка данных аудиторий {index} выглядит неполной или является пустой");
                            cond = false;
                        }

                    }
                    else if (user_password == old_user.password || user_login == old_user.login
                            || user_access == old_user.access.ToString().ToLower() || user_name == old_user.name)
                    {
                        users.Cells.SetCellValue(index - 1, 0, new_user.login);
                        users.Cells.SetCellValue(index - 1, 1, new_user.password);
                        users.Cells.SetCellValue(index - 1, 2, new_user.access.ToString().ToLower());
                        users.Cells.SetCellValue(index - 1, 3, new_user.name);
                        break;
                    }
                    index++;
                }

                if (cond)
                {
                    FileInfo excelFile = new(fullPath);
                    excel.SaveAs(excelFile);
                }
                return cond;

            }
            else
            {
                LogState("Не получилось изменить запись (возможно, заменяемого вами пользователя не существует)");
                return false;
            }
        }
        public bool SetDesk(Desk old_desk, Desk new_desk)///сменить одну доску на другую 
        {
            //открываем файл с данными 
            string fullPath = data_path;
            ExcelPackage excel = new(new FileInfo(fullPath));

            //задаём списки 
            ExcelWorksheet? desks = excel.Workbook.Worksheets["Доски"];

            if (desks == null)
            {
                LogState("Пересмотри вводимые тобой данные кабинетов");
                if (consoleLogging)
                {
                    Console.WriteLine("Нажми кнопку для выхода");
                    _ = Console.ReadKey();
                }
                return false;
            }

            int indexToIns = -1;
            if (this.desks.FindIndex(x => x == old_desk) != -1)
            {
                indexToIns = this.desks.FindIndex(x => x == old_desk);
            }
            else
            {
                LogState("Не получилось изменить запись (возможно, заменяемой вами аудитории не существует)");
                return false;
            }


            bool cond = true;
            if (this.desks.Remove(old_desk))
            {
                this.desks.Insert(indexToIns, new_desk);
                int index = 1;
                while (index <= desks.Dimension.End.Row)
                {
                    string? codeName = desks.Cells[$"A{index}"].Value?.ToString();
                    string? startTime = desks.Cells[$"B{index}"].Value?.ToString().Split(' ')[1];
                    string? endTime = desks.Cells[$"C{index}"].Value?.ToString().Split(' ')[1];
                    string? capacity = desks.Cells[$"D{index}"].Value?.ToString();
                    if (codeName == null || startTime == null || endTime == null || capacity == null)
                    {
                        if (codeName == null && startTime == null && endTime == null && capacity == null)
                        {
                        }
                        else
                        {
                            cond = false;
                            LogState($"Строка данных пользователя {index} выглядит неполной");
                        }
                    }
                    else if (old_desk.id == codeName && old_desk.startTime + ":00" == startTime && old_desk.endTime + ":00" == endTime
                            && old_desk.capacity == int.Parse(capacity))
                    {
                        desks.Cells.SetCellValue(index - 1, 0, new_desk.id);
                        desks.Cells["B1"].Value = Date("01.01.2000 " + new_desk.startTime);
                        desks.Cells["B1"].Style.Numberformat.Format = "H:mm";
                        desks.Cells["C1"].Value = Date("01.01.2000 " + new_desk.endTime);
                        desks.Cells["C1"].Style.Numberformat.Format = "H:mm";
                        desks.Cells.SetCellValue(index - 1, 3, new_desk.capacity);
                        break;
                    }
                    index++;
                }
            }
            else
            {
                LogState("Не получилось изменить запись (возможно, заменяемой вами аудитории не существует)");
                return false;
            }

            if (cond)
            {
                FileInfo excelFile = new(fullPath);
                excel.SaveAs(excelFile);
            }
            return cond;
        }

        public bool DeleteCard(Card delete_card)///удалить карточку 
        {
            Desk desk = delete_card.desk;
            if (desk.timetable.Remove(delete_card))
            {
                string filePath;

                try
                {
                    filePath = cards_path + delete_card.startTime.ToString("yyyy.MM.dd") + ".desk"; // путь к файлу
                }
                catch (Exception ex) { LogState($"Ошибка: {ex}"); return false; }
                bool sucess = false;

                /*Название предмета 1 | 9:00 | 10:00 | Преподаватель 1 | Доп описание для Название предмета 1 1 | a1*/
                string searchLine = delete_card.name + '|' + delete_card.startTime.ToString("H:mm") + '|' + delete_card.endTime.ToString("H:mm") + '|'
                                    + delete_card.teacher.name + '|' + delete_card.subname + '|' + delete_card.desk.id;// строка, которую нужно заменить

                // Открываем файл для чтения и записи
                try
                {

                    using StreamReader reader = new(filePath);
                    // Создаем временный файл для записи
                    string tempFilePath = System.IO.Path.GetTempFileName();

                    // Открываем временный файл для записи
                    using (StreamWriter writer = new(tempFilePath))
                    {
                        string line;
                        string? temp_line = string.Empty;
                        bool lineFound = false;


                        // Читаем файл построчно
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (lineFound)
                            {
                                if (temp_line != string.Empty)
                                {
                                    writer.WriteLine(temp_line);
                                }

                                lineFound = false;

                            }

                            if (line.Contains(searchLine))
                            {
                                lineFound = true;
                                sucess = true;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    if (line.Split('|').Length > 2)
                                    {
                                        temp_line = line;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                writer.WriteLine(line);
                            }
                        }

                        if (lineFound)
                        {
                            if (temp_line != string.Empty)
                            {
                                writer.WriteLine(temp_line);
                            }

                            lineFound = false;

                        }

                        // Если строка не была найдена
                        if (!sucess)
                        {
                            LogState($"Строка для замены \"{searchLine}\" не найдена");
                            sucess = false;
                        }
                    }

                    if (sucess)
                    {
                        // Закрываем файлы
                        reader.Close();

                        // Заменяем исходный файл временным файлом
                        File.Delete(filePath);
                        File.Move(tempFilePath, filePath);
                        sucess = true;
                    }

                }
                catch (IOException ex)
                {
                    LogState("Возникла следующая ошибка: " + ex.Message);
                    sucess = false;
                }
                return sucess;
            }
            else
            {
                LogState("Не получилось изменить запись (возможно, заменяемой вами записи не существует)");
                return false;
            }
        }
        public bool DeleteUser(User delete_user)///удалить пользователя 
        {
            string fullPath = users_path;
            ExcelPackage excel = new(new FileInfo(fullPath));
            ExcelWorksheet? users = excel.Workbook.Worksheets["Данные"];
            if (users == null)
            {
                LogState("Пересмотри данные пользователей");
                return false;
            }

            if (this.users.Contains(delete_user))
            {
                int index = 1;
                bool cond = true;
                while (index <= users.Dimension.End.Row)
                {
                    string? user_login = users.Cells[$"A{index}"].Value?.ToString();
                    string? user_password = users.Cells[$"B{index}"].Value?.ToString();
                    string? user_access = users.Cells[$"C{index}"].Value?.ToString();
                    string? user_name = users.Cells[$"D{index}"].Value?.ToString();
                    if (user_password == null || user_login == null || user_access == null || user_name == null)
                    {
                        if (user_password == null && user_login == null && user_access == null && user_name == null)
                        {

                        }
                        else
                        {
                            LogState($"Строка данных пользователя {index} выглядит неполной");
                            cond = false;
                        }
                    }
                    else if (user_password == delete_user.password && user_login == delete_user.login
                            && user_access == delete_user.access.ToString().ToLower() && user_name == delete_user.name)
                    {
                        users.DeleteRow(index);
                        break;
                    }
                    else
                    {
                        index++;
                    }
                }

                if (cond)
                {
                    FileInfo excelFile = new(fullPath);
                    excel.SaveAs(excelFile);
                    /*if (delete_user.access == Access.Teacher)
                    {
                        DeleteTeacher(delete_user.name);
                    }
                    else
                    {*/
                    foreach (Desk desk in desks)
                    {
                        for (int i = 0; i < desk.timetable.Count; i++)
                        {
                            if (desk.timetable[i].participators.Contains(delete_user))
                            {
                                Card card0 = new(desk.timetable[i]);
                                card0.participators.Remove(delete_user);
                                SetCard(desk.timetable[i], card0);
                                this.users.Remove(delete_user);
                                i--;
                            }

                        }
                    }
                    /*}*/
                }

                return cond;
            }

            else
            {
                LogState("Не получилось изменить запись (возможно, заменяемого вами пользователя не существует)");
                return false;
            }
        }
        public bool DeleteDesk(Desk delete_desk)///удалить доску 
        {
            //открываем файл с данными 
            string fullPath = data_path;
            ExcelPackage excel = new(new FileInfo(fullPath));

            //задаём списки 
            ExcelWorksheet? desks = excel.Workbook.Worksheets["Доски"];

            if (desks == null)
            {
                LogState("Пересмотри вводимые тобой данные кабинетов");
                if (consoleLogging)
                {
                    Console.WriteLine("Нажми кнопку для выхода");
                    _ = Console.ReadKey();
                }
                return false;
            }

            bool cond = true;
            if (GetDesk(delete_desk.id) != null)
            {
                int index = 1;
                while (index <= desks.Dimension.End.Row)
                {
                    string? codeName = desks.Cells[$"A{index}"].Value?.ToString();
                    DateTime timeValue;

                    string? startTime;
                    ExcelRange startTime0 = desks.Cells[$"B{index}"];
                    try
                    {
                        timeValue = DateTime.FromOADate(startTime0.GetValue<double>());
                        startTime = timeValue.ToString("H:mm");
                    }
                    catch (Exception)
                    {
                        timeValue = Date(startTime0.GetValue<string>());
                        startTime = timeValue.ToString("H:mm");

                    }


                    string? endTime;
                    ExcelRange endTime0 = desks.Cells[$"C{index}"];
                    try
                    {
                        timeValue = DateTime.FromOADate(endTime0.GetValue<double>());
                        endTime = timeValue.ToString("H:mm");
                    }
                    catch (Exception)
                    {
                        timeValue = Date(endTime0.GetValue<string>());
                        endTime = timeValue.ToString("H:mm");

                    }
                    string? capacity = desks.Cells[$"D{index}"].Value?.ToString();


                    if (codeName == null || startTime == null || endTime == null || capacity == null)
                    {
                        if (codeName == null && startTime == null && endTime == null && capacity == null)
                        {
                        }
                        else
                        {
                            LogState($"Строка данных аудитории {index} выглядит неполной");
                            cond = false;
                        }
                    }
                    else if (delete_desk.id == codeName && delete_desk.startTime == startTime && delete_desk.endTime == endTime
                            && delete_desk.capacity == int.Parse(capacity))
                    {
                        desks.DeleteRow(index);
                        break;
                    }
                    else
                    {
                        index++;
                    }
                }
            }
            else
            {
                LogState("Не получилось изменить запись (возможно, заменяемой вами аудитории не существует)");
                return false;
            }

            if (cond)
            {
                FileInfo excelFile = new(fullPath);
                excel.SaveAs(excelFile);
                delete_desk = GetDesk(delete_desk.id);
                for (int i = 0; i < this.desks.Count; i++)
                {
                    if (delete_desk == this.desks[i])
                    {
                        for (int j = 0; j < this.desks[i].timetable.Count; j++)
                        {
                            Card card = this.desks[i].timetable[j];
                            if (card.desk.id == delete_desk.id)
                            {
                                DeleteCard(card);
                            }
                        }
                        this.desks.RemoveAt(i);
                    }
                }
            }

            return cond;
        }

        public bool AddCard(Card new_card)///добавить карточку 
        {
            Desk desk = GetDesk(new_card.desk.id);
            if (desk.AddCard(new_card, this))
            {
                using StreamWriter writer = new(CreateDayList(new_card.startTime.ToString("yyyy.MM.dd")), true);
                string newLine = new_card.name + '|' + new_card.startTime.ToString("H:mm") + '|' + new_card.endTime.ToString("H:mm") + '|'
                        + new_card.teacher.name + '|' + new_card.subname + '|' + new_card.desk.id; // новая строка, которой заменится найденная строка
                writer.WriteLine(newLine);
                return true;
            }
            else
            {
                LogState($"Добавление записи \"{new_card.name}\" безуспешно завершено");
                return false;
            }
        }
        public bool AddUser(User new_user)///добавить пользователя 
        {
            string fullPath = users_path;
            ExcelPackage excel = new(new FileInfo(fullPath));
            ExcelWorksheet? users = excel.Workbook.Worksheets["Данные"];
            if (users == null)
            {
                LogState("Пересмотри данные пользователей");
                return false;
            }

            int index = 1;
            bool cond = false;
            bool right = true;
            while (index <= users.Dimension.End.Row)
            {
                string? user_login = users.Cells[$"A{index}"].Value?.ToString();
                string? user_password = users.Cells[$"B{index}"].Value?.ToString();
                string? user_access = users.Cells[$"C{index}"].Value?.ToString();
                string? user_name = users.Cells[$"D{index}"].Value?.ToString();
                if (user_password == null || user_login == null || user_access == null || user_name == null)
                {
                    if (user_password == null && user_login == null && user_access == null && user_name == null)
                    {
                        users.Cells.SetCellValue(index - 1, 0, new_user.login);
                        users.Cells.SetCellValue(index - 1, 1, new_user.password);
                        users.Cells.SetCellValue(index - 1, 2, new_user.access.ToString().ToLower());
                        users.Cells.SetCellValue(index - 1, 3, new_user.name);
                        cond = true;
                    }
                    else
                    {
                        LogState($"Строка данных пользователя {index} выглядит неполной или является пустой");
                        right = false;
                    }
                }
                index++;
            }
            if (!cond)
            {
                users.Cells.SetCellValue(index - 1, 0, new_user.login);
                users.Cells.SetCellValue(index - 1, 1, new_user.password);
                users.Cells.SetCellValue(index - 1, 2, new_user.access.ToString().ToLower());
                users.Cells.SetCellValue(index - 1, 3, new_user.name);
            }

            if (right)
            {
                FileInfo excelFile = new(fullPath);
                excel.SaveAs(excelFile);
            }
            return right;
        }
        public bool AddDesk(Desk new_desk)///добавить доску 
        {
            //открываем файл с данными 
            string fullPath = data_path;
            ExcelPackage excel = new(new FileInfo(fullPath));

            //задаём списки 
            ExcelWorksheet? desks = excel.Workbook.Worksheets["Доски"];

            if (desks == null)
            {
                LogState("Пересмотри вводимые тобой данные кабинетов");
                if (consoleLogging)
                {
                    Console.WriteLine("Нажми кнопку для выхода");
                    _ = Console.ReadKey();
                }
                return false;
            }

            int index = 1;
            bool cond = false;
            bool right = true;
            while (index <= desks.Dimension.End.Row)
            {
                string? codeName = desks.Cells[$"A{index}"].Value?.ToString();
                DateTime timeValue;

                string? startTime;
                ExcelRange startTime0 = desks.Cells[$"B{index}"];
                try
                {
                    timeValue = DateTime.FromOADate(startTime0.GetValue<double>());
                    startTime = timeValue.ToString("H:mm");
                }
                catch (Exception)
                {
                    timeValue = Date(startTime0.GetValue<string>());
                    startTime = timeValue.ToString("H:mm");

                }


                string? endTime;
                ExcelRange endTime0 = desks.Cells[$"C{index}"];
                try
                {
                    timeValue = DateTime.FromOADate(endTime0.GetValue<double>());
                    endTime = timeValue.ToString("H:mm");
                }
                catch (Exception)
                {
                    timeValue = Date(endTime0.GetValue<string>());
                    endTime = timeValue.ToString("H:mm");

                }
                string? capacity = desks.Cells[$"D{index}"].Value?.ToString();
                if (codeName == null || startTime == null || endTime == null || capacity == null)
                {
                    if (codeName == null && startTime == null && endTime == null && capacity == null)
                    {
                        index++;
                        desks.Cells[$"A{index}"].Value = new_desk.id;
                        desks.Cells.SetCellValue(index - 1, 1, Date("01.01.2000 " + new_desk.startTime));
                        desks.Cells[$"B{index}"].Style.Numberformat.Format = "H:mm";
                        desks.Cells.SetCellValue(index - 1, 2, Date("01.01.2000 " + new_desk.endTime));
                        desks.Cells[$"C{index}"].Style.Numberformat.Format = "H:mm";
                        desks.Cells[$"D{index}"].Value = new_desk.capacity;
                        cond = true;
                    }
                    else
                    {
                        LogState($"Строка данных аудитории {index} выглядит неполной");
                        right = false;
                    }
                }
                index++;
            }
            if (!cond)
            {
                desks.Cells[$"A{index}"].Value = new_desk.id;
                desks.Cells.SetCellValue(index - 1, 1, Date("01.01.2000 " + new_desk.startTime));
                desks.Cells[$"B{index}"].Style.Numberformat.Format = "H:mm";
                desks.Cells.SetCellValue(index - 1, 2, Date("01.01.2000 " + new_desk.endTime));
                desks.Cells[$"C{index}"].Style.Numberformat.Format = "H:mm";
                desks.Cells[$"D{index}"].Value = new_desk.capacity;
            }

            if (right)
            {
                FileInfo excelFile = new(fullPath);
                excel.SaveAs(excelFile);
            }
            return right;
        }


        ////////////////////////////////////////////////////////////////////////////////////////
        ///////////////Базовые функции, не требующиеся в дальнейшем использовании///////////////
        public DataBase(string logfile_path, bool consoleLogging, string progname, string version)//конструктор 
        {
            log_counter = 0;
            this.version = version;
            this.progname = progname;
            desks = new();
            users = new();
            this.logfile_path = logfile_path;
            this.consoleLogging = consoleLogging;
        }
        enum CardReadState { ReadCard, ReadCheck }

        public bool FillUsers(string pathToFileUsers)//первоначальное заполнение всех пользователей 
        {
            //открываем файл с данными 
            string fullPath = pathToFileUsers;
            users_path = fullPath;
            ExcelPackage excel = new(new FileInfo(fullPath));
            ExcelWorksheet? users = excel.Workbook.Worksheets["Данные"];
            if (users == null)
            {
                LogState("Пересмотри вводимые тобой данные пользователей");
                if (consoleLogging)
                {
                    Console.WriteLine("Нажми кнопку для выхода");
                    Console.ReadKey();
                }
                return false;
            }

            int index = 1;
            bool cond = true;
            while (index <= users.Dimension.End.Row)
            {
                string? user_id = users.Cells[$"A{index}"].Value?.ToString();
                string? user_login = users.Cells[$"B{index}"].Value?.ToString();
                string? user_password = users.Cells[$"C{index}"].Value?.ToString();
                string? user_name = users.Cells[$"D{index}"].Value?.ToString();
                if (user_login == null || user_id == null || user_password == null || user_name == null)
                {
                    if (user_login == null && user_id == null && user_password == null && user_name == null) { users.DeleteRow(index); }
                    else
                    {
                        LogState($"Строка данных пользователя {index} выглядит неполной или является пустой");
                        cond = false;
                    }
                }
                else
                {
                    this.users.Add(new(user_id, user_login, user_password, user_name, this));
                    index++;
                }
            }
            return cond;
        }
        public bool FillData(string pathToFileData)//заполнение списка досок с заданиями 
        {
            //открываем файл с данными 
            string fullPath = pathToFileData;
            data_path = fullPath;
            ExcelPackage excel = new(new FileInfo(fullPath));
            bool right = true;

            //задаём списки 
            ExcelWorksheet? desks = excel.Workbook.Worksheets["Доски"];

            if (desks == null)
            {
                LogState("Пересмотри вводимые тобой данные кабинетов");
                if (consoleLogging)
                {
                    Console.WriteLine("Нажми кнопку для выхода");
                    Console.ReadKey();
                }
                return false;
            }

            int index = 1;
            while (index <= desks.Dimension.End.Row)
            {
                string? desk_id = desks.Cells[$"A{index}"].Value?.ToString();
                string? desk_name = desks.Cells[$"B{index}"].Value?.ToString();
                string? desk_type = desks.Cells[$"C{index}"].Value?.ToString();
                string? user_id = desks.Cells[$"D{index}"].Value?.ToString();

                if (desk_id == null || desk_name == null || desk_type == null || user_id == null)
                {
                    if (desk_id == null && desk_name == null && desk_type == null && user_id == null) { desks.DeleteRow(index); }
                    else if(desk_name.Contains('|'))
                    {
                        LogState($"Строка данных доски заданий номер {index} имеет недопустимый символ");
                        right = false;
                    }
                    else
                    {
                        LogState($"Строка данных доски заданий номер {index} выглядит неполной");
                        right = false;
                    }
                }
                else
                {
                    int type;
                    if (GetUser(user_id) == null) right = false;
                    else if (!int.TryParse(desk_type, out type)) right = false;
                    else if (Desk.GetType(int.Parse(desk_type)) == Type.Error) right = false;
                    else this.desks.Add(new(desk_name, GetUser(user_id), Desk.GetType(int.Parse(desk_type)), this));
                    index++;
                }
            }
            return right;
        }
        public bool FillDays(string pathToDirDays)//первоначальное заполнение всех броней 
        {
            cards_path = pathToDirDays;
            string[] files = Directory.GetFiles(pathToDirDays, "*.desk");
            Card? temp_card = new(this, false);
            Desk? temp_desk = new(this, false);
            CardReadState state = CardReadState.ReadCard;
            bool result = true;

            foreach (string fileName in files)
            {
                string name = System.IO.Path.GetFileName(fileName).Split(".desk")[0];
                using StreamReader reader = new(fileName);
                if (reader.EndOfStream)
                {
                    LogState($"Файл \".../data/cards/{name}\" пуст");
                    result = false;
                }

                else
                {
                    string line;
                    bool cond = false;
                    bool falseCard = true;
                    bool falseCheck = true;
                    int rowNum = 1;

                    string temp_cardID = String.Empty;

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[]? parametrs = line.Split("|");

                        if (parametrs.Length == 4)
                        {
                            string[] dta = data_path.Split("//");
                            state = CardReadState.ReadCard;
                            foreach (Desk desk in desks)
                            {
                                if (dta[^1] == desk.id)
                                {
                                    temp_card = new(parametrs[0], parametrs[1], Card.GetDone(parametrs[3]), parametrs[2], this);
                                    temp_desk = desk;
                                    desk.cards.Add(new(temp_card, this, false));
                                    cond = true; falseCard = false;
                                    break;
                                }
                            }
                        }

                        else if (parametrs.Length == 3 && state == CardReadState.ReadCard)
                        {
                            temp_cardID = parametrs[1];
                            if (parametrs[0] != "*")
                            {
                                LogState($"Целостность строки {rowNum} нарушена, желательно перепроверить");
                            }
                            bool cond1 = false;
                            state = CardReadState.ReadCheck;
                            foreach (Desk desk in desks)
                            {
                                if (cond1) break;
                                foreach(Card card in desk.cards)
                                {
                                    if (parametrs[1] == card.id)
                                    {
                                        temp_card = card;
                                        temp_desk = desk;
                                        card.checkList = new(parametrs[1], Check.GetDone(parametrs[2]));
                                        cond = true; falseCard = false; cond1 = true;
                                        break;
                                    }
                                }
                            }
                            falseCheck = cond1;
                        }

                        else if (parametrs.Length == 3 && state == CardReadState.ReadCheck)
                        {
                            state = CardReadState.ReadCard;
                        }

                        else if (parametrs.Length == 2 && !falseCard && state == CardReadState.ReadCard)
                        {
                            User? temp_user = GetUser(parametrs[1]);
                            if (temp_desk == null || temp_card == null)
                            {
                                LogState($"Прочтение строки {line} безуспешно завершено. Проверьте информацию в файле {name + ".desk"} в строке {rowNum}");
                                result = false;
                            }
                            else if (temp_user == null)
                            {
                                string[] dta = data_path.Split("//");
                                LogState($"Взятие пользователя по строке {line} безуспешно завершено. Проверьте информацию в файле {name + ".desk"} в строке {rowNum} и файле {dta[^1]}");
                                result = false;
                            }
                            else if (temp_desk.type == Type.Private)
                            {
                                string[] dta = data_path.Split("//");
                                LogState($"Считывание строки номер {rowNum} с пользователем проигнорирована, т.к. доска определена как приватная");
                                result = false;
                            }
                            else if (temp_desk.type == Type.Error)
                            {
                                string[] dta = data_path.Split("//");
                                LogState($"Считывание файла с данными доски {temp_desk.id} невозможно, перепроверьте данные в {data_path.Split("//")[^1]}");
                                result = false;
                            }
                            else
                            {
                                temp_desk.cards.Add(temp_card);
                            }
                        }

                        else if (parametrs.Length == 2 && !falseCard && state == CardReadState.ReadCheck && !falseCheck)
                        {
                            if (temp_desk == null || temp_card == null)
                            {
                                LogState($"Прочтение строки {line} безуспешно завершено. Проверьте информацию в файле {name + ".desk"} в строке {rowNum}");
                                result = false;
                            }
                            else if (parametrs[0] == null || parametrs[1] == null)
                            {
                                string[] dta = data_path.Split("//");
                                LogState($"Взятие пользователя по строке {line} безуспешно завершено. Проверьте информацию в файле {name + ".desk"} в строке {rowNum} и файле {dta[dta.Length - 1]}");
                                result = false;
                            }
                            else
                            {
                                temp_card.checkList.tasks.Add(new(parametrs[0]) { done = Card.GetDone(parametrs[1])});
                            }
                        }
                        else if (parametrs == null || falseCard || falseCheck) { }
                        else
                        {
                            LogState($"Неверный формат данных в строке {line}");
                            result = false;
                        }
                        rowNum++;
                    }

                    if (!cond) result = false;
                }
            }
            return result;
        }
        
        public bool CreateDataList(string fileName)//создание макета списка данных 
        {
            //создаем новый документ 
            ExcelPackage excel = new();

            //добавляем лист 
            ExcelWorksheet worksheet3 = excel.Workbook.Worksheets.Add("Доски");

            //добавляем данные 

            worksheet3.Cells["A1"].Value = "Идентификатор";
            worksheet3.Column(1).Width = 30;
            worksheet3.Cells["B1"].Value = "Имя доски";
            worksheet3.Column(2).Width = 30;
            worksheet3.Cells["C1"].Value = "Тип доски";
            worksheet3.Column(3).Width = 30;
            worksheet3.Cells["D1"].Value = "Идентификатор создателя";
            worksheet3.Column(4).Width = 30;

            //задаём путь 
            string fullPath = fileName;

            //сохраняем документ 
            FileInfo excelFile = new(fullPath);
            if (!File.Exists(fullPath)) { excel.SaveAs(excelFile); return false; }
            else
            {
                return false;
            }
        }
        public bool CreateUserList(string fileName)//создание макета списка юзеров 
        {
            //создаем новый документ 
            ExcelPackage excel = new();

            //добавляем лист 
            ExcelWorksheet worksheet1 = excel.Workbook.Worksheets.Add("Данные");

            //добавляем данные 
            worksheet1.Cells["A1"].Value = "Идентификатор";
            worksheet1.Column(1).Width = 30;

            worksheet1.Cells["B1"].Value = "Логин";
            worksheet1.Column(2).Width = 30;

            worksheet1.Cells["C1"].Value = "Пароль";
            worksheet1.Column(3).Width = 30;

            worksheet1.Cells["D1"].Value = "ОТображаемое имя";
            worksheet1.Column(4).Width = 30;

            //задаём путь 
            string fullPath = fileName;

            //сохраняем документ 
            FileInfo excelFile = new(fullPath);
            if (!File.Exists(fullPath)) { excel.SaveAs(excelFile); return false; }
            else
            {
                return true;
            }
        }
    }
}