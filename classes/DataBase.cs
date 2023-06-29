using Microsoft.VisualBasic.ApplicationServices;
using OfficeOpenXml;

namespace TaskManager
{
    public class DataBase
    {
        public List<Desk> desks;//доски с заданиями 
        public List<User> users;//пользователи 

        public List<string> user_ids;//все идентификаторы пользователей 
        public List<string> desk_ids;//все идентификаторы досок 
        public List<string> card_ids;//все идентификаторы карточек 

        public string logfile_path; //путь к папке с логами 
        public string data_path; //путь к таблице с данными 
        public string users_path; //путь к таблице с юзерами 
        public string desks_path; //путь к папке с досками 
        private readonly bool consoleLogging;//делать логи в консоли или нет (выключить, если нужно будет работать с визуализацией) 

        public Logger logState; //форма для логирования программы 
        private int log_counter; //для LogState
        public string version { get; }//версия программы 
        public string progname { get; }//имя программы 

        ////////////////////Переменные, необходимые для работы всей датабазы////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////

        //некоторые вспомогательные инструменты
        public static DateTime Date(string date)//для дат формата "dd.MM.yyyy hh:mm" (при первой неудаче форматирует как "yyyy.MM.dd hh:mm")
        {
            string[] datenums = date.Split(' ')[0].Split('.');
            string[] timenums = date.Split(' ')[1].Split(':');
            string error = string.Empty;
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
            string fullPath = desks_path + card_id + ".desk";

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

            using (StreamWriter writer = new(dirWithLogName, true))
            {
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
                        /*logState.LogWindow.AppendText(line + " ЛОГ ЗАПУСКА " + DateTime.Now.ToString("HH:mm:ss.fff") + " " + line + '\n');*/
                    }
                }
                writer.Write(DateTime.Now.ToString("HH:mm:ss.fff") + " : " + message + '\n');
                if (consoleLogging)
                {
                    /*if (message.Contains('\n')) logState.LogWindow.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + " : " + message);
                    else logState.LogWindow.AppendText(DateTime.Now.ToString("HH:mm:ss.fff") + " : " + message + '\n');*/
                }
            }
            logState.LogUpdate();
        }
        int boolConvert(bool b)
        {
            if (b) return 1;
            else return 0;
        }
        public User RandLogPass(string name)//создание автоматически сгенерированного пользователя, но не регистрирует его в датабазе(!)
        {
            // Создание генератора случайных чисел
            Random random = new Random();

            // Создание случайного логина
            string login = "user" + user_ids.Count + 1;

            // Создание случайного пароля
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string password = new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());

            LogState($"(bC1) Успешно создан в памяти пользователь с следующими данными (логин|пароль) : {login}|{password}");
            return new(login, password, name, this);
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
                LogState($"(GFU1) Пользователя с именем \"{name}\" и логином \"{login}\" нету в базе данных");
            }

            return null;
        }



        //рабочие инструменты базы данных
        public Task? GetTask(string desk_id, string card_id, string name)//получение пункта в чек-листе 
        {
            bool cond = true;
            if (GetDesk(desk_id) == null)
            {
                LogState($"(GT1) Доска с идентификатором {desk_id} не найдена");
                cond = false;
            }
            if (GetCard(card_id) == null)
            {
                LogState($"(GT2) Карта с идентификатором {card_id} для доски с идентификатором {desk_id} не найдена");
                cond = false;
            }
            else if (GetCard(card_id).checkList == null)
            {
                LogState($"(GT3) Для карты с идентификатором {card_id} в доске с идентификатором {desk_id} не найден чек-лист");
                cond = false;
            }
            if (cond == true) foreach (Task task in desks.Find(x => x.id == desk_id).cards.Find(x => x.id == card_id).checkList.tasks)
                {
                    if (task.name == name) return task;
                }
            LogState($"(GT4) Попытка найти пункт с именем {name} чек-листа карточки с идентификатором {card_id} доски с идентификатором {desk_id} не вышло");
            return null;
        }
        public bool DeleteTask(Task delete_task)//удаление пункта в чек-листе 
        {
            bool cond = false;

            Card? old_task_card = GetCard(delete_task.card_id);

            string fileTaskOld = desks_path + old_task_card.desk_id + ".desk";

            try
            {
                string ftold_context = File.ReadAllText(fileTaskOld);

                if (desks.Find(x => x.id == old_task_card.desk_id).cards.Find(x => x.id == delete_task.card_id).checkList.tasks.Remove(delete_task))
                {
                    List<string> lines = ftold_context.Split('\n').ToList();
                    bool c = false;
                    bool incheck = false;
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (c)
                        {
                            if (lines[i].Split('|').Length == 3)
                            {
                                incheck = !incheck;
                            }
                        }

                        if (incheck)
                        {
                            if (lines[i].Split('|')[0].Trim() == delete_task.name)
                            {
                                lines.RemoveAt(i); cond = true; break;
                            }
                        }

                        if (lines[i].Split('|').Length == 4)
                        {
                            if (lines[i].Split('|')[0] == delete_task.card_id)
                            {
                                c = true;
                            }
                        }
                    }
                    ftold_context = string.Join('\n', lines.ToArray());
                }
                else
                {
                    LogState($"(DT1) Удаляемый пункт карточки \"{delete_task.card_id}\" с именем \"{delete_task.name}\"не найден");
                    cond = false;
                }

                if (cond)
                {
                    File.WriteAllText(fileTaskOld, ftold_context);
                }
            }
            catch (Exception ex)
            {
                LogState($"(DT2) Возникла следующая ошибка: {ex.Message}");
            }

            return cond;
        }
        public bool AddTask(Task new_task)//добавление пункта в чек-листе 
        {
            bool cond = false;
            Card? old_task_card = GetCard(new_task.card_id);
            string fileTaskOld = desks_path + old_task_card.desk_id + ".desk";

            try
            {
                string ftold_context = File.ReadAllText(fileTaskOld);

                if (desks.Find(x => x.id == old_task_card.desk_id).cards.Find(x => x.id == new_task.card_id) != null)
                {
                    List<string> lines = ftold_context.Split('\n').ToList();
                    bool c = false;
                    bool incheck = false;
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (c)
                        {
                            if (lines[i].Split('|').Length == 3)
                            {
                                incheck = !incheck;
                                if (!incheck)
                                {
                                    lines.Insert(i, $"{new_task.name}|{boolConvert(new_task.done)}");
                                    cond = true;
                                    break;
                                }
                            }
                        }

                        if (lines[i].Split('|').Length == 4)
                        {
                            if (lines[i].Split('|')[0] == new_task.card_id)
                            {
                                c = true;
                            }
                            else c = false;
                        }
                    }
                    ftold_context = string.Join('\n', lines.ToArray());
                }
                else
                {
                    LogState($"(AT1) Карточка \"{new_task.card_id}\" не найдена для добавления пункта");
                    cond = false;
                }
                if (cond)
                {
                    File.WriteAllText(fileTaskOld, ftold_context);
                    desks.Find(x => x.id == old_task_card.desk_id).cards.Find(x => x.id == new_task.card_id).checkList.tasks.Add(new_task);
                }
            }
            catch (Exception ex)
            {
                LogState($"(AT2) Возникла следующая ошибка: {ex.Message}");
            }

            return cond;
        }
        public bool MoveTask(Task move_task, int zero_position)///смещение позиции пункта в чек-листе 
        { return true; }

        public Card? GetCard(string id)//найти карточку по идентификатору 
        {
            foreach (Desk desk in desks)
            {
                foreach (Card card in desk.cards)
                {
                    if (card.id == id)
                    {
                        return card;
                    }
                }
            }
            LogState($"(GC1) Не найдено задания с идентификатором \"{id}\"");
            return null;
        }
        public bool DeleteCard(Card delete_card)//удалить карточку 
        {
            Desk? desk = GetDesk(delete_card.desk_id);
            if (GetCard(delete_card.id) == null)
            {
                LogState("(DC1) Произошла непредвиденная ошибка поиска карточки. Перепроверьте все вводимые данные");
                return false;
            }
            else if (desk == null)
            {
                LogState("(DC2) Произошла непредвиденная ошибка поиска доски. Перепроверьте все вводимые данные");
                return false;
            }
            else
            {
                if (desks.Find(x => x.id == delete_card.desk_id).cards.Remove(GetCard(delete_card.id)))
                {
                    desk_ids.Remove(delete_card.id);
                    /*desk.cards.Add(new_card);*/
                    string filePath;
                    try
                    {
                        filePath = desks_path + delete_card.desk_id + ".desk"; //путь к файлу 
                    }
                    catch (Exception ex) { LogState($"(DC3) Возникла следующая ошибка: {ex.Message}"); return false; }

                    /*eoa71BFthJHwA6iY|задача 1|нужно что-то там сделать (зачемто)|0*/
                    string searchLine = delete_card.id + '|' + delete_card.name + '|' + delete_card.info + '|' + boolConvert(delete_card.done);//строка, которую нужно заменить 

                    //открываем файл для чтения и записи 
                    try
                    {
                        using StreamReader reader = new(filePath);
                        //создаем временный файл для записи 
                        string tempFilePath = System.IO.Path.GetTempFileName();

                        //открываем временный файл для записи 
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
                                    while (temp_line != null && temp_line.Split('|').Length != 4)
                                    {
                                            temp_line = reader.ReadLine();
                                    }
                                    writer.WriteLine(temp_line);
                                    lineFound = false;
                                    sucess = true;
                                }

                                else if (searchLine == line)
                                {
                                    lineFound = true;
                                }
                                else
                                {
                                    writer.WriteLine(line);
                                }
                            }

                            // Если строка не была найдена
                            if (!sucess)
                            {
                                LogState($"(DC4) Строка для удаления \"{searchLine.Split('\n')[0]}\" не найдена");
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
                        LogState("(DC5) Возникла следующая ошибка: " + ex.Message);
                        return false;
                    }

                    return true;
                }
                else
                {
                    LogState("(DC6) Не получилось удалить карточку (возможно, заменяемой вами записи не существует)");
                    return false;
                }
            }
        }
        public bool AddCard(Card new_card)//добавить карточку 
        {
            Desk? desk = GetDesk(new_card.desk_id);
            if (desk == null)
            {
                LogState("(AC1) Произошла непредвиденная ошибка поиска, дальнейшая смена карточки невозможна. Перепроверьте все вводимые данные");
                return false;
            }
            else
            {
                
                string filePath;
                try
                {
                    filePath = desks_path + new_card.desk_id + ".desk"; //путь к файлу 
                }
                catch (Exception ex) { LogState($"(AC2) Возникла следующая ошибка: {ex.Message}"); return false; }

                /*eoa71BFthJHwA6iY|задача 1|нужно что-то там сделать (зачемто)|0*/

                string newLine = new_card.id + '|' + new_card.name + '|' + new_card.info + '|' + boolConvert(new_card.done) + '\n'; //новая строка, которой заменится найденная строка 
                newLine += "*|" + new_card.id + '|' + boolConvert(new_card.checkList.done) + '\n';
                foreach (TaskManager.Task task in new_card.checkList.tasks)
                {
                    newLine += task.name + '|' + boolConvert(task.done) + '\n';
                }
                newLine += "*|" + new_card.id + '|' + boolConvert(new_card.checkList.done) + '\n';


                //открываем файл для чтения и записи 
                try
                {
                    using StreamReader reader = new(filePath);
                    //создаем временный файл для записи 
                    string tempFilePath = System.IO.Path.GetTempFileName();

                    //открываем временный файл для записи 
                    using (StreamWriter writer = new(tempFilePath))
                    {
                        string line;
                        // Читаем файл построчно
                        while ((line = reader.ReadLine()) != null) writer.WriteLine(line);
                        foreach (string str in newLine.Split('\n')) writer.WriteLine(str);
                    }

                    // Закрываем файлы
                    reader.Close();

                    // Заменяем исходный файл временным файлом
                    File.Delete(filePath);
                    File.Move(tempFilePath, filePath);

                    desks.Find(x => x.id == new_card.desk_id).cards.Add(new(new_card, this, false));
                }
                catch (IOException ex)
                {
                    LogState("(AC3) Возникла следующая ошибка: " + ex.Message);
                    return false;
                }

                return true;
            }
        }
        public bool MoveCard(Card move_card, int zero_position)///смещение позиции карточки на доске 
        { return true; }

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
                LogState($"(GU1) Пользователя с идентификатором \"{id}\" нету в базе данных");
            }
            return null;
        }
        public bool DeleteUser(User delete_user)//удалить пользователя 
        {
            string fullPath = users_path;
            ExcelPackage excel = new(new FileInfo(fullPath));
            ExcelWorksheet? users = excel.Workbook.Worksheets["Данные"];

            if (users == null)
            {
                LogState("(DU1) Пересмотри данные пользователей");
                return false;
            }

            int indexToIns = -1;
            if (this.users.FindIndex(x => x.id == delete_user.id) != -1)
            {
                indexToIns = this.users.FindIndex(x => x.id == delete_user.id);
            }
            else
            {
                LogState("(DU2) Не получилось изменить пользователя");
                return false;
            }

            if (this.users.Remove(GetUser(delete_user.id)))
            {
                user_ids.Remove(delete_user.id);
                /*this.users.Insert(indexToIns, new_user);*/
                int index = 1;
                bool cond = true;
                while (index <= users.Dimension.End.Row)
                {
                    string? user_id = users.Cells[$"A{index}"].Value?.ToString();
                    string? user_login = users.Cells[$"B{index}"].Value?.ToString();
                    string? user_pass = users.Cells[$"C{index}"].Value?.ToString();
                    string? user_name = users.Cells[$"D{index}"].Value?.ToString();
                    if (user_login == null || user_id == null || user_pass == null || user_name == null)
                    {
                        if (user_login == null && user_id == null && user_pass == null && user_name == null) { users.DeleteRow(index); }
                        else
                        {
                            LogState($"(DU3) Строка данных пользователей \"{index}\" выглядит неполной");
                            cond = false;
                        }

                    }
                    else if (user_id == delete_user.id) { users.DeleteRow(index); }
                    index++;
                }

                List<string> fileTaskOld = new();
                foreach (string desk_id in delete_user.owner)
                {
                    fileTaskOld.Add(desks_path + @"\" + desk_id + ".desk");
                }
                foreach (string desk_id in delete_user.admin)
                {
                    fileTaskOld.Add(desks_path + @"\" + desk_id + ".desk");
                }
                foreach (string desk_id in delete_user.guest)
                {
                    fileTaskOld.Add(desks_path + @"\" + desk_id + ".desk");
                }

                fileTaskOld = fileTaskOld.Distinct().ToList();

                if (cond) try
                {
                    foreach (string path in fileTaskOld)
                    {
                        string ftold_context = File.ReadAllText(path);

                        if (desk_ids.Contains(path.Split('\\')[^1].Split(".desk")[0]))
                        {
                            List<string> lines = ftold_context.Split('\n').ToList();
                            bool c = false;
                            for (int i = 0; i < lines.Count; i++)
                            {
                                if (lines[i].Split('|').Length == 3) c = !c;
                                else if (lines[i].Split('|').Length == 4) c = true;
                                else if (lines[i].Split('|').Length == 2 && c)
                                {
                                    if (lines[i].Trim().Split('|')[1] == delete_user.id)
                                    {
                                        lines.RemoveAt(i);
                                    }
                                }
                            }
                            ftold_context = string.Join('\n', lines.ToArray());
                        }
                        else
                        {
                            LogState($"(DU4) Удаляемый пользователь \"{delete_user.id}\" не найден");
                            cond = false;
                        }

                        if (cond)
                        {
                            File.WriteAllText(path, ftold_context);
                        }
                    }

                }
                catch (Exception ex)
                {
                    LogState($"(DU5) Возникла следующая ошибка: {ex.Message}");
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
                LogState("(DU6) Не получилось удалить пользователя");
                return false;
            }
        }
        public bool AddUser(User new_user)//добавить пользователя 
        {
            string fullPath = users_path;
            ExcelPackage excel = new(new FileInfo(fullPath));
            ExcelWorksheet? users = excel.Workbook.Worksheets["Данные"];

            if (users == null)
            {
                LogState("(AU1) Пересмотри данные пользователей");
                return false;
            }

            int index = 1;
            bool cond = true;
            while (index <= users.Dimension.End.Row)
            {
                string? user_id = users.Cells[$"A{index}"].Value?.ToString();
                string? user_login = users.Cells[$"B{index}"].Value?.ToString();
                string? user_pass = users.Cells[$"C{index}"].Value?.ToString();
                string? user_name = users.Cells[$"D{index}"].Value?.ToString();
                if (user_login == null || user_id == null || user_pass == null || user_name == null)
                {
                    if (user_login == null && user_id == null && user_pass == null && user_name == null) { users.DeleteRow(index); }
                    else
                    {
                        LogState($"(AU2) Строка данных пользователей \"{index}\" выглядит неполной или является пустой");
                        cond = false;
                    }

                }
                index++;
            }
            users.Cells.SetCellValue(index - 1, 0, new_user.id);
            users.Cells.SetCellValue(index - 1, 1, new_user.login);
            users.Cells.SetCellValue(index - 1, 2, new_user.password);
            users.Cells.SetCellValue(index - 1, 3, new_user.name);

            try
            {
                List<string> fileTaskOld = new();

                fileTaskOld.Add("***");
                foreach (string desk_id in new_user.owner)
                {
                    fileTaskOld.Add(desks_path + @"\" + desk_id + ".desk");
                }
                fileTaskOld.Add("**");
                foreach (string desk_id in new_user.admin)
                {
                    fileTaskOld.Add(desks_path + @"\" + desk_id + ".desk");
                }
                fileTaskOld.Add("*");
                foreach (string desk_id in new_user.guest)
                {
                    fileTaskOld.Add(desks_path + @"\" + desk_id + ".desk");
                }

                U stat = U.User;

                if (cond) try
                    {
                        foreach (string path in fileTaskOld)
                        {
                            if (path == "*") stat = U.User;
                            else if (path == "**") stat = U.Admin;
                            else if (path == "***") stat = U.Owner;
                            else
                            {
                                string ftold_context = File.ReadAllText(path);

                                if (desk_ids.Contains(path.Split('\\')[^1].Split(".desk")[0]))
                                {
                                    List<string> lines = ftold_context.Split('\n').ToList();
                                    for (int i = 0; i < lines.Count; i++)
                                    {
                                        if (lines[i].Split('|').Length == 4) lines.Insert(i + 1, $"{(int)stat}|{new_user.id}");
                                    }
                                    ftold_context = string.Join('\n', lines.ToArray());
                                }
                                else
                                {
                                    LogState($"(AU3) Поиск данных для обновления доски \"{path.Split('\\')[^1].Split(".desk")[0]}\" безуспешно завершён");
                                    cond = false;
                                }

                                if (cond)
                                {
                                    File.WriteAllText(path, ftold_context);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        LogState($"(AU4) Возникла следующая ошибка: {ex.Message}");
                    }
            }
            catch (Exception ex)
            {
                LogState($"(AU5) Возникла следующая ошибка: {ex.Message}");
            }


            if (cond)
            {
                this.users.Add(new(new_user, this, false));
                FileInfo excelFile = new(fullPath);
                excel.SaveAs(excelFile);
            }
            return cond;

        }
        public bool MoveUser(User move_user, int zero_position)///смещение позиции пользователя в списке всех пользователей 
        { return true; }

        public Desk? GetDesk(string id)//найти доску по идентификатору 
        {
            foreach (Desk desk in desks)
            {
                if (desk.id == id)
                {
                    return desk;
                }
            }
            LogState($"(GD1) Аудитория с номером \"{id}\" не найдена");
            return null;
        }
        public bool DeleteDesk(Desk delete_desk)//удалить доску 
        {
            //открываем файл с данными 
            string fullPath = data_path;
            ExcelPackage excel = new(new FileInfo(fullPath));

            //задаём списки 
            ExcelWorksheet? desks = excel.Workbook.Worksheets["Доски"];

            if (desks == null)
            {
                LogState($"(DD1) Пересмотри вводимые тобой данные досок в файле \"{data_path.Split("\\")[^1]}\"");
                return false;
            }

            int indexToIns = -1;
            if (this.desks.FindIndex(x => x.id == delete_desk.id) != -1)
            {
                indexToIns = this.desks.FindIndex(x => x.id == delete_desk.id);
            }
            else
            {
                LogState("(DD2) Не получилось изменить запись (возможно, заменяемой вами аудитории не существует)");
                return false;
            }


            bool cond = true;
            if (this.desks.Remove(GetDesk(delete_desk.id)))
            {
                /*this.desks.Insert(indexToIns, new_desk);*/
                desk_ids.Remove(delete_desk.id);
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
                        else
                        {
                            cond = false;
                            LogState($"(DD3) Строка данных пользователя \"{index}\" выглядит неполной");
                        }
                    }

                    else if (delete_desk.id == desk_id) { desks.DeleteRow(index); desk_ids.Remove(delete_desk.id); }
                    index++;
                }

                string fileDeskOld = desks_path + delete_desk.id + ".desk";
                File.Delete(fileDeskOld);
            }
            else
            {
                LogState("(DD4) Не получилось изменить запись (возможно, заменяемой вами аудитории не существует)");
                return false;
            }

            if (cond)
            {
                FileInfo excelFile = new(fullPath);
                excel.SaveAs(excelFile);
            }
            return cond;

        }
        public bool AddDesk(Desk new_desk)//добавить доску 
        {
            //открываем файл с данными 
            string fullPath = data_path;
            ExcelPackage excel = new(new FileInfo(fullPath));

            //задаём списки 
            ExcelWorksheet? desks = excel.Workbook.Worksheets["Доски"];

            if (desks == null)
            {
                LogState($"(DD5) Пересмотри вводимые тобой данные досок в файле \"{data_path.Split("\\")[^1]}\"");
                return false;
            }

            bool cond = true;
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
                    else
                    {
                        cond = false;
                        LogState($"Строка данных пользователя \"{index}\" выглядит неполной");
                    }
                }
                index++;
            }
            desks.Cells.SetCellValue(index - 1, 0, new_desk.id);
            desks.Cells.SetCellValue(index - 1, 1, new_desk.name);
            desks.Cells.SetCellValue(index - 1, 2, (int)new_desk.type);
            desks.Cells.SetCellValue(index - 1, 3, new_desk.owner.id);

            string fileDeskNew = desks_path + new_desk.id + ".desk";
            if(!File.Exists(fileDeskNew)) File.Create(fileDeskNew);

            List<string> context = new();

            foreach(var card in new_desk.cards)
            {
                context.Add($"{card.id}|{card.name}|{card.info}|{boolConvert(card.done)}");
                foreach(var user in users)
                {
                    if(user.owner.Find(x => x == new_desk.id) != null) context.Add($"2|{user.id}");
                    if (user.admin.Find(x => x == new_desk.id) != null) context.Add($"1|{user.id}");
                    if (user.guest.Find(x => x == new_desk.id) != null) context.Add($"0|{user.id}");
                }
                if(card.checkList != null)
                {
                    context.Add($"*|{card.name}|{card.id}|{boolConvert(card.checkList.done)}");
                    foreach (var task in card.checkList.tasks)
                    {
                        context.Add($"{task.name}|{boolConvert(task.done)}");
                    }
                    context.Add($"*|{card.name}|{card.id}|{boolConvert(card.checkList.done)}");
                }
            }
            
            File.WriteAllText(fileDeskNew, string.Join('\n', context.ToArray()));

            /* using (StreamWriter writer = new(fileDeskNew))
             {
                 foreach (string id in desk_ids)
                 {
                     if (GetDesk(id) == null) { LogState($"Доска с идентификатором {id} не найдена"); }
                     else
                     {
                         foreach (Card card in this.desks.Find(x => x.id == id).cards)
                         {
                             writer.WriteLine($"{card.id}|{card.name}|{card.info}|{boolConvert(card.done)}");
                             if (GetDesk(id).type == Type.Private) { }
                             else if (GetDesk(id).type == Type.Public)
                             {
                                 foreach (string uid in user_ids)
                                 {
                                     if (GetUser(uid) == null) LogState($"Пользователь с идентификатором {id} для доски с идентификатором {id} не найден");
                                     else
                                     {
                                         if (GetUser(uid).owner.Contains(new_desk.id)) writer.WriteLine($"2|{uid}");
                                         if (GetUser(uid).admin.Contains(new_desk.id)) writer.WriteLine($"1|{uid}");
                                         if (GetUser(uid).guest.Contains(new_desk.id)) writer.WriteLine($"0|{uid}");
                                     }
                                 }
                             }
                             else { LogState($"Доска с идентификатором {id} имеет ошибочный тип"); return false; }

                             if (card.checkList == null) { }
                             else
                             {
                                 string edge = $"*|{card.id}|{boolConvert(card.checkList.done)}";
                                 writer.WriteLine(edge);
                                 foreach (Task task in card.checkList.tasks)
                                 {
                                     writer.WriteLine($"{task.name}|{boolConvert(task.done)}");
                                 }
                                 writer.WriteLine(edge);
                             }
                         }
                     }
                 }
             }*/

            if (cond)
            {
                FileInfo excelFile = new(fullPath);
                excel.SaveAs(excelFile);
            }
            return cond;
        }
        public bool MoveDesk(Desk move_desk, int zero_position)///смещение позиции доски в списке всех досок 
        { return true; }

        ////////////////////////////////////////////////////////////////////////////////////////
        ///////////////Базовые функции, не требующиеся в дальнейшем использовании///////////////
        enum CardReadState { ReadCard, ReadCheck }
        public DataBase(string logfile_path, bool consoleLogging, Logger log, string progname, string version)//конструктор 
        {
            log_counter = 0;
            this.version = version;
            this.progname = progname;
            logState = log;
            logState.db = this;
            desks = new();
            users = new();
            user_ids = new();
            card_ids = new();
            desk_ids = new();
            this.logfile_path = logfile_path;
            this.consoleLogging = consoleLogging;
        }

        public bool FillUsers(string pathToFileUsers)//первоначальное заполнение всех пользователей 
        {
            //открываем файл с данными 
            string fullPath = pathToFileUsers;
            users_path = fullPath;
            ExcelPackage excel = new(new FileInfo(fullPath));
            ExcelWorksheet? users = excel.Workbook.Worksheets["Данные"];
            if (users == null)
            {
                LogState("(FU1) Пересмотри вводимые тобой данные пользователей");
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
                        LogState($"(FU2) Строка данных пользователя \"{index}\" выглядит неполной или является пустой");
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
                LogState("(FData1) Пересмотри вводимые тобой данные кабинетов");
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
                    else if (desk_name.Contains('|'))
                    {
                        LogState($"(FData2) Строка данных доски заданий номер \"{index}\" имеет недопустимый символ");
                        right = false;
                    }
                    else
                    {
                        LogState($"(FData3) Строка данных доски заданий номер \"{index}\" выглядит неполной");
                        right = false;
                    }
                }
                else
                {
                    int type;
                    if (GetUser(user_id) == null) { LogState($"(FData4) Пользователь \"{user_id}\" не найден"); right = false; }
                    else if (!int.TryParse(desk_type, out type)) { LogState($"(FData5) Попытки преобразовать модификатор типа \"{desk_type}\" для доски \"{desk_id}\" обречён провалом"); right = false; }
                    else if (Desk.GetType(int.Parse(desk_type)) == Type.Error) { LogState($"(FData6) Ошибочный модификатор \"{desk_type}\" для доски \"{desk_id}\", возвращено Type.Error"); right = false; }
                    else this.desks.Add(new(desk_id, desk_name, GetUser(user_id), Desk.GetType(int.Parse(desk_type)), this));
                    index++;
                }
            }
            return right;
        }
        public bool FillDesks(string pathToDirDays)//первоначальное заполнение всех броней 
        {
            desks_path = pathToDirDays;
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
                    LogState($"(FDesks1) Файл \"...\\{pathToDirDays.Split("\\")[^2]}\\{name}.desk\" пуст");
                }

                else
                {
                    string line;
                    bool cond = false;
                    bool falseCard = true;
                    bool falseCheck = false;
                    int rowNum = 1;

                    string temp_cardID = string.Empty;

                    while ((line = reader.ReadLine()) != null)
                    {
                        string[]? parametrs = line.Split("|");

                        if (parametrs.Length == 4)
                        {
                            falseCheck = false;
                            if (falseCheck)
                            {
                                LogState($"(FDesks2) Отсутствует чек-лист карточки \"{temp_card.id}\" в файле {name}.desk");
                            }
                            string[] dta = data_path.Split("//");
                            state = CardReadState.ReadCard;
                            foreach (Desk desk in desks)
                            {
                                if (name == desk.id)
                                {
                                    temp_card = new(parametrs[0], desk.id, parametrs[1], Card.GetDone(parametrs[3]), parametrs[2], this);
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
                                LogState($"(Fdesks3) Целостность строки \"{rowNum}\" файла \"{name}.desk\" нарушена, желательно перепроверить");
                                falseCheck = true;
                            }
                            else
                            {
                                falseCheck = false;
                                bool cond1 = false;
                                state = CardReadState.ReadCheck;
                                foreach (Desk desk in desks)
                                {
                                    if (cond1) break;
                                    foreach (Card card in desk.cards)
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
                            }
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
                                LogState($"(FDesks4) Прочтение строки \"{line}\" безуспешно завершено. Проверьте информацию в файле \"{name + ".desk"}\" в строке \"{rowNum}\"");
                                result = false;
                            }
                            else if (temp_user == null)
                            {
                                string[] dta = data_path.Split("//");
                                LogState($"(FDesks5) Взятие пользователя по строке \"{line}\" безуспешно завершено. Проверьте информацию в файле \"{name + ".desk"}\" в строке \"{rowNum}\" и файле \"{dta[^1]}\"");
                                result = false;
                            }
                            else if (temp_desk.type == Type.Private)
                            {
                                string[] dta = data_path.Split("//");
                                LogState($"(FDesks6) Считывание строки номер \"{rowNum}\" с пользователем проигнорирована, т.к. доска \"{name}\" карточки \"{temp_card.id}\" определена как приватная");
                                /*result = false;*/
                            }
                            else if (temp_desk.type == Type.Error)
                            {
                                string[] dta = data_path.Split("//");
                                LogState($"(FDesks7) Считывание файла с данными доски \"{temp_desk.id}\" невозможно, перепроверьте данные в \"{data_path.Split("//")[^1]}\"");
                                result = false;
                            }
                            else
                            {
                                switch (parametrs[0])
                                {
                                    case "0":
                                        if (!users.Find(x => x.id == parametrs[1]).guest.Contains(temp_desk.id)) users.Find(x => x.id == parametrs[1]).guest.Add(temp_desk.id);
                                        break;
                                    case "1":
                                        if (!users.Find(x => x.id == parametrs[1]).admin.Contains(temp_desk.id)) users.Find(x => x.id == parametrs[1]).admin.Add(temp_desk.id);
                                        break;
                                    case "2":
                                        if (!users.Find(x => x.id == parametrs[1]).owner.Contains(temp_desk.id)) users.Find(x => x.id == parametrs[1]).owner.Add(temp_desk.id);
                                        break;
                                    default:
                                        LogState($"(FDesks8) Неправильный вид доступа пользователя в строке. Проверьте информацию в файле \"{name}.desk\" в строке \"{rowNum}\"");
                                        break;
                                }
                            }
                        }

                        else if (parametrs.Length == 2 && !falseCard && state == CardReadState.ReadCheck && !falseCheck)
                        {
                            if (temp_desk == null || temp_card == null)
                            {
                                LogState($"(FDesks9) Прочтение строки \"{line}\" безуспешно завершено. Проверьте информацию в файле \"{name + ".desk"}\" в строке \"{rowNum}\"");
                                result = false;
                            }
                            else if (parametrs[0] == null || parametrs[1] == null)
                            {
                                string[] dta = data_path.Split("//");
                                LogState($"(FDesks10) Взятие пользователя по строке \"{line}\" безуспешно завершено. Проверьте информацию в файле \"{name + ".desk"}\" в строке \"{rowNum}\" и файле \"{dta[^1]}\"");
                                result = false;
                            }
                            else
                            {
                                temp_card.checkList.tasks.Add(new(parametrs[0], temp_card.id) { done = Card.GetDone(parametrs[1]) });
                            }
                        }
                        else if (parametrs == null || falseCard || falseCheck || line.Trim() == "") { }
                        else
                        {
                            LogState($"(FDesks11) Неверный формат данных в файле \"{name + ".desk"}\" в строке \"{rowNum}\"");
                            result = false;
                        }
                        rowNum++;
                    }

                    if (!cond)
                    {
                        LogState("(FDesks12) Не удалось обработать ни одной карточки");
                        result = false;
                    }
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
                LogState($"(CDL1) Файл \"{fileName.Split("\\")[^1]}\" уже существует");
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
                LogState($"(CUL1) Файл \"{fileName.Split("\\")[^1]}\" уже существует");
                return true;
            }
        }

        enum U { Owner = 2, Admin = 1, User = 0 }

        /*public bool SetDesk(Desk old_desk, Desk new_desk)//сменить одну доску на другую 
        {
            //открываем файл с данными 
            string fullPath = data_path;
            ExcelPackage excel = new(new FileInfo(fullPath));

            //задаём списки 
            ExcelWorksheet? desks = excel.Workbook.Worksheets["Доски"];

            if (desks == null)
            {
                LogState($"(SD1) Пересмотри вводимые тобой данные досок в файле \"{data_path.Split("\\")[^1]}\"");
                return false;
            }

            int indexToIns = -1;
            if (this.desks.FindIndex(x => x.id == old_desk.id) != -1)
            {
                indexToIns = this.desks.FindIndex(x => x.id == old_desk.id);
            }
            else
            {
                LogState("(SD2) Не получилось изменить запись (возможно, заменяемой вами аудитории не существует)");
                return false;
            }


            bool cond = true;
            if (this.desks.Remove(GetDesk(old_desk.id)))
            {
                this.desks.Insert(indexToIns, new_desk);
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
                        else
                        {
                            cond = false;
                            LogState($"(SD3) Строка данных пользователя \"{index}\" выглядит неполной");
                        }
                    }

                    else if (old_desk.id == desk_id)
                    {
                        desks.Cells.SetCellValue(index - 1, 0, new_desk.id);
                        desks.Cells.SetCellValue(index - 1, 1, new_desk.name);
                        desks.Cells.SetCellValue(index - 1, 2, (int)new_desk.type);
                        desks.Cells.SetCellValue(index - 1, 3, new_desk.owner.id);
                        break;
                    }
                    index++;
                }

                string fileDeskOld = desks_path + old_desk.id + ".desk";
                string fileDeskNew = desks_path + new_desk.id + ".desk";
                File.Delete(fileDeskOld);
                File.Create(fileDeskNew);
                using (StreamWriter writer = new(fileDeskNew))
                {
                    foreach (string id in desk_ids)
                    {
                        if (GetDesk(id) == null) LogState($"(SD4) Доска с идентификатором {id} не найдена");
                        else
                        {
                            foreach (Card card in this.desks.Find(x => x.id == id).cards)
                            {
                                writer.WriteLine($"{card.id}|{card.name}|{card.info}|{boolConvert(card.done)}");
                                if (GetDesk(id).type == Type.Private) { }
                                else if (GetDesk(id).type == Type.Public)
                                {
                                    foreach (string uid in user_ids)
                                    {
                                        if (GetUser(uid) == null) LogState($"(SD5) Пользователь с идентификатором {id} для доски с идентификатором {id} не найден");
                                        else
                                        {
                                            if (GetUser(uid).owner.Contains(new_desk.id)) writer.WriteLine($"2|{uid}");
                                            if (GetUser(uid).admin.Contains(new_desk.id)) writer.WriteLine($"1|{uid}");
                                            if (GetUser(uid).guest.Contains(new_desk.id)) writer.WriteLine($"0|{uid}");
                                        }
                                    }
                                }
                                else { LogState($"(SD6) Доска с идентификатором {id} имеет ошибочный тип"); return false; }

                                if (card.checkList == null) { }
                                else
                                {
                                    string edge = $"*|{card.id}|{boolConvert(card.checkList.done)}";
                                    writer.WriteLine(edge);
                                    foreach (Task task in card.checkList.tasks)
                                    {
                                        writer.WriteLine($"{task.name}|{boolConvert(task.done)}");
                                    }
                                    writer.WriteLine(edge);
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                LogState("(SD7) Не получилось изменить запись (возможно, заменяемой вами аудитории не существует)");
                return false;
            }

            if (cond)
            {
                FileInfo excelFile = new(fullPath);
                excel.SaveAs(excelFile);
            }
            return cond;
        }*/
        /*public bool SetCard(Card old_card, Card new_card)//сменить одну карточку на другую 
        {
            Desk? desk = GetDesk(old_card.desk_id);
            if (GetCard(old_card.id) == null)
            {
                LogState($"(SC1) Заменяемая карточка \"{old_card.id}\" не найдена. Перепроверьте все вводимые данные");
                return false;
            }
            else if (desk == null)
            {
                LogState($"(SC2) Доска заменяемой карточки \"{old_card.desk_id}\" не найдена. Перепроверьте все вводимые данные");
                return false;
            }
            else
            {
                if (this.desks.Find(x => x.id == old_card.desk_id).cards.Remove(GetCard(old_card.id)))
                {
                    this.desks.Find(x => x.id == old_card.desk_id).cards.Add(new_card);
                    string filePath;
                    try
                    {
                        filePath = desks_path + old_card.desk_id + ".desk"; //путь к файлу 
                    }
                    catch (Exception ex) { LogState($"(SC3) Возникла следующая ошибка: {ex.Message}"); return false; }

                    eoa71BFthJHwA6iY | задача 1 | нужно что - то там сделать(зачемто)| 0
                    string searchLine = old_card.id + '|' + old_card.name + '|' + old_card.info + '|' + boolConvert(old_card.done) + '\n';//строка, которую нужно заменить 
                    searchLine += "*|" + old_card.id + '|' + boolConvert(old_card.checkList.done) + '\n';
                    foreach (TaskManager.Task task in old_card.checkList.tasks)
                    {
                        searchLine += task.name + '|' + boolConvert(task.done) + '\n';
                    }
                    searchLine += "*|" + old_card.id + '|' + boolConvert(old_card.checkList.done) + '\n';

                    string newLine = new_card.id + '|' + new_card.name + '|' + new_card.info + '|' + boolConvert(new_card.done) + '\n'; //новая строка, которой заменится найденная строка 
                    newLine += "*|" + new_card.id + '|' + boolConvert(new_card.checkList.done) + '\n';
                    foreach (TaskManager.Task task in new_card.checkList.tasks)
                    {
                        newLine += task.name + '|' + boolConvert(task.done) + '\n';
                    }
                    newLine += "*|" + new_card.id + '|' + boolConvert(new_card.checkList.done);


                    //открываем файл для чтения и записи 
                    try
                    {
                        using StreamReader reader = new(filePath);
                        //создаем временный файл для записи 
                        string tempFilePath = System.IO.Path.GetTempFileName();

                        //открываем временный файл для записи 
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
                                    writer.WriteLine(newLine);
                                    lineFound = false;
                                    sucess = true;
                                }

                                if (searchLine.Contains(line))
                                {
                                    lineFound = true;
                                    while (!searchLine.Contains(line))
                                    {
                                        reader.ReadLine();
                                    }

                                }
                                else
                                {
                                    writer.WriteLine(line);
                                }
                            }

                            // Если строка не была найдена
                            if (!sucess)
                            {
                                LogState($"(SC4) Строка для замены \"{searchLine.Split('\n')[0]}\" не найдена");
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
                        LogState("(SC5) Возникла следующая ошибка: " + ex.Message);
                        return false;
                    }

                    return true;
                }
                else
                {
                    LogState("(SC6) Не получилось изменить карточку (возможно, заменяемой вами записи не существует)");
                    return false;
                }
            }

        }*/
        /*public bool SetTask(Task old_task, Task new_task)//замена пункта в чек-листе 
        {
            bool cond = true;

            var old_task_card = GetCard(old_task.card_id);
            var new_task_card = GetCard(new_task.card_id);

            string fileTaskNew = desks_path + new_task_card.desk_id + ".desk";
            string fileTaskOld = desks_path + old_task_card.desk_id + ".desk";

            string ftnew_context = File.ReadAllText(fileTaskNew);
            string ftold_context = File.ReadAllText(fileTaskOld);

            if (desks.Find(x => x.id == old_task_card.desk_id).cards.Find(x => x.id == old_task.card_id).checkList.tasks.Remove(old_task))
            {
                ftold_context.Replace($"{old_task.name}|{boolConvert(old_task.done)}\n", String.Empty);
            }
            else
            {
                LogState($"(ST1) Заменяемый пункт карточки \"{old_task.card_id}\" не найден");
                cond = false;
            }

            if (!ftnew_context.Contains($"{new_task.name}|{boolConvert(new_task.done)}"))
            {
                desks.Find(x => x.id == new_task_card.desk_id).cards.Find(x => x.id == new_task.card_id).checkList.tasks.Add(new_task);
                var ftnew_lines = ftnew_context.Split('\n');
                bool find = false;
                bool cond1 = false;
                int count = 0;
                foreach( var line in ftnew_lines )
                {
                    if (cond1 && !find) break;
                    string[] paramts = line.Split('|');

                    if (paramts.Length == 3 && paramts[0] == "*" && paramts[1] == new_task.card_id )
                    {
                        find = !find;
                    }
                    if (find) cond1 = true;

                    count++;
                }
                var cntxt = ftnew_lines.ToList();
                cntxt.Insert(count, $"{new_task.name}|{boolConvert(new_task.done)}\n");
                ftnew_context = string.Join('\n', cntxt.ToArray());
            }
            else
            {
                LogState($"(ST2) Новый пункт карточки \"{new_task.card_id}\" уже существует");
                cond = false;
            }

            if ( cond )
            {
                File.WriteAllText(fileTaskOld, ftold_context);
                File.WriteAllText(fileTaskNew, ftnew_context);
            }
            return cond;
        }*/
        /* public bool SetUser(User old_user, User new_user)//сменить одного пользователя на другого 
        {
            string fullPath = users_path;
            ExcelPackage excel = new(new FileInfo(fullPath));
            ExcelWorksheet? users = excel.Workbook.Worksheets["Данные"];

            if (users == null)
            {
                LogState("(SU1) Пересмотри данные пользователей");
                return false;
            }

            int indexToIns = -1;
            if (this.users.FindIndex(x => x.id == old_user.id) != -1)
            {
                indexToIns = this.users.FindIndex(x => x.id == old_user.id);
            }
            else
            {
                LogState("(SU2) Не получилось изменить запись (возможно, заменяемого вами пользователя не существует)");
                return false;
            }

            if (this.users.Remove(GetUser(old_user.id)))
            {
                this.users.Insert(indexToIns, new_user);
                int index = 1;
                bool cond = true;
                while (index <= users.Dimension.End.Row)
                {
                    string? user_id = users.Cells[$"A{index}"].Value?.ToString();
                    string? user_login = users.Cells[$"B{index}"].Value?.ToString();
                    string? user_pass = users.Cells[$"C{index}"].Value?.ToString();
                    string? user_name = users.Cells[$"D{index}"].Value?.ToString();
                    if (user_login == null || user_id == null || user_pass == null || user_name == null)
                    {
                        if (user_login == null && user_id == null && user_pass == null && user_name == null) { users.DeleteRow(index); }
                        else
                        {
                            LogState($"(SU3) Строка данных пользователей \"{index}\" выглядит неполной или является пустой");
                            cond = false;
                        }

                    }
                    else if (user_id == old_user.id)
                    {
                        users.Cells.SetCellValue(index - 1, 0, new_user.id);
                        users.Cells.SetCellValue(index - 1, 1, new_user.login);
                        users.Cells.SetCellValue(index - 1, 2, new_user.password);
                        users.Cells.SetCellValue(index - 1, 3, new_user.name);
                        break;
                    }
                    index++;
                }

                foreach (string id in old_user.owner)
                {
                    string filePath = desks_path + id + ".desk";
                    if (File.Exists(filePath))
                    {
                        string tempFilePath = System.IO.Path.GetTempFileName();
                        using (StreamReader reader = new(filePath))
                        {
                            using (StreamWriter writer = new(tempFilePath))
                            {
                                string line = reader.ReadLine();
                                if (line.Split('|').Count() == 2)
                                {
                                    if (line.Split('|')[1] == old_user.id) { }
                                    else writer.WriteLine(line);
                                }
                                else writer.WriteLine(line);
                            }
                        }
                        File.Delete(filePath);
                        File.Move(tempFilePath, filePath);
                    }
                    else
                    {
                        LogState($"(SU4) Файла с информацией о доске {id} нету в директории с досками");
                    }
                }
                foreach (string id in old_user.admin)
                {
                    string filePath = desks_path + id + ".desk";
                    if (File.Exists(filePath))
                    {
                        string tempFilePath = System.IO.Path.GetTempFileName();
                        using (StreamReader reader = new(filePath))
                        {
                            using (StreamWriter writer = new(tempFilePath))
                            {
                                string line = reader.ReadLine();
                                if (line.Split('|').Count() == 2)
                                {
                                    if (line.Split('|')[1] == old_user.id) { }
                                    else writer.WriteLine(line);
                                }
                                else writer.WriteLine(line);
                            }
                        }
                        File.Delete(filePath);
                        File.Move(tempFilePath, filePath);
                    }
                    else
                    {
                        LogState($"(SU5) Файла с информацией о доске {id} нету в директории с досками");
                    }
                }
                foreach (string id in old_user.guest)
                {
                    string filePath = desks_path + id + ".desk";
                    if (File.Exists(filePath))
                    {
                        string tempFilePath = System.IO.Path.GetTempFileName();
                        using (StreamReader reader = new(filePath))
                        {
                            using (StreamWriter writer = new(tempFilePath))
                            {
                                string line = reader.ReadLine();
                                if (line.Split('|').Count() == 2)
                                {
                                    if (line.Split('|')[1] == old_user.id) { }
                                    else writer.WriteLine(line);
                                }
                                else writer.WriteLine(line);
                            }
                        }
                        File.Delete(filePath);
                        File.Move(tempFilePath, filePath);
                    }
                    else
                    {
                        LogState($"(SU6) Файла с информацией о доске {id} нету в директории с досками");
                    }
                }

                foreach (string id in new_user.owner)
                {
                    string filePath = desks_path + id + ".desk";
                    if (File.Exists(filePath))
                    {
                        string tempFilePath = System.IO.Path.GetTempFileName();
                        using (StreamReader reader = new(filePath))
                        {
                            using (StreamWriter writer = new(tempFilePath))
                            {
                                string line = reader.ReadLine();
                                if (line.Split('|').Count() == 4)
                                {
                                    writer.WriteLine(line);
                                    writer.WriteLine($"2|{new_user.id}");
                                }
                                else writer.WriteLine(line);
                            }
                        }
                        File.Delete(filePath);
                        File.Move(tempFilePath, filePath);
                    }
                    else
                    {
                        LogState($"(SU7) Файла с информацией о доске {id} нету в директории с досками");
                    }
                }
                foreach (string id in new_user.admin)
                {
                    string filePath = desks_path + id + ".desk";
                    if (File.Exists(filePath))
                    {
                        string tempFilePath = System.IO.Path.GetTempFileName();
                        using (StreamReader reader = new(filePath))
                        {
                            using (StreamWriter writer = new(tempFilePath))
                            {
                                string line = reader.ReadLine();
                                if (line.Split('|').Count() == 4)
                                {
                                    writer.WriteLine(line);
                                    writer.WriteLine($"1|{new_user.id}");
                                }
                                else writer.WriteLine(line);
                            }
                        }
                        File.Delete(filePath);
                        File.Move(tempFilePath, filePath);
                    }
                    else
                    {
                        LogState($"(SU8) Файла с информацией о доске {id} нету в директории с досками");
                    }
                }
                foreach (string id in new_user.guest)
                {
                    string filePath = desks_path + id + ".desk";
                    if (File.Exists(filePath))
                    {
                        string tempFilePath = System.IO.Path.GetTempFileName();
                        using (StreamReader reader = new(filePath))
                        {
                            using (StreamWriter writer = new(tempFilePath))
                            {
                                string line = reader.ReadLine();
                                if (line.Split('|').Count() == 4)
                                {
                                    writer.WriteLine(line);
                                    writer.WriteLine($"0|{new_user.id}");
                                }
                                else writer.WriteLine(line);
                            }
                        }
                        File.Delete(filePath);
                        File.Move(tempFilePath, filePath);
                    }
                    else
                    {
                        LogState($"(SU9) Файла с информацией о доске {id} нету в директории с досками");
                    }
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
                LogState("(SU10) Не получилось изменить карточку (возможно, заменяемого вами пользователя не существует)");
                return false;
            }
        }*/

        ////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////     Какая-то шлёпа из методов    /////////////////////////////
        public void ShowCards(Desk desk)
        {
            for (int i = 0; i < desk.cards.Count; i++)
            {
                LogState(i + ".   " + desk.cards[i].name);
            }
        }
        public List<Card> CardsReturn(Desk desk)
        {
            List<Card> cards = new List<Card>();
            for (int i = 0; i < desk.cards.Count; i++)
            {
                cards.Add(desk.cards[i]);
            }
            return cards;
        }
        public Check TasksReturnCard(Card card)
        {
            Check check = card.checkList;
            return check;
        }
        public List<Task> TasksReturnCheck(Check check)
        {
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < check.tasks.Count; i++)
            {
                tasks.Add(check.tasks[i]);
                LogState(i + ".   " + check.tasks[i].name);
            }
            return tasks;
        }
        public string createId()
        {
            Random random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            string rnd = new string(Enumerable.Repeat(chars, 11).Select(s => s[random.Next(s.Length)]).ToArray());
            return rnd;
        }

        public void TaskBoolChangerToTrue(Task task)
        {
            Card card;
            Desk desk;
            card = GetCard(task.card_id);
            desk = GetDesk(card.desk_id);

            string deskString = desk.id + ".desk";
            string path = "data\\desks\\" + deskString;

            var lines = File.ReadAllLines(path).ToList();

            string itemSearch = task.name + "|0" ;
            string itemChange = task.name + "|1" ;

            int index = lines.FindIndex(s => s.Contains(itemSearch));
            if (index != -1)
            {
                lines[index] = lines[index].Replace(itemSearch, itemChange);
                File.WriteAllLines(path, lines);
                task.done = true;
            }

/*            if (lines.FindIndex(s => s.Contains(itemSearch)) != null)
            {
                int index = lines.FindIndex(s => s.Contains(itemSearch));

                lines[index] = lines[index].Replace(itemSearch, itemChange);

                File.WriteAllLines(path, lines);
            }*/
        }
        public void TaskBoolChangerToFalse(Task task)
        {
            Card card;
            Desk desk;
            card = GetCard(task.card_id);
            desk = GetDesk(card.desk_id);

            string deskString = desk.id + ".desk";
            string path = "data\\desks\\" + deskString;

            var lines = File.ReadAllLines(path).ToList();

            string itemSearch = task.name + "|1";
            string itemChange = task.name + "|0";

            int index = lines.FindIndex(s => s.Contains(itemSearch));
            if (index != -1)
            {
                lines[index] = lines[index].Replace(itemSearch, itemChange);
                File.WriteAllLines(path, lines);
                task.done = false;
            }
        }
        public void ChangeNameCard(Card card, string nameNew)
        {
            Desk desk;
            desk = GetDesk(card.desk_id);
            string deskString = desk.id + ".desk";
            string path = "data\\desks\\" + deskString;
            var lines = File.ReadAllLines(path).ToList();
            int doneInt;
            if (card.done == true) doneInt = 1;
            else doneInt = 0;

            string itemSearch = card.id + "|" + card.name + "|" + card.info + "|" + doneInt;
            string itemChange = card.id + "|" + nameNew + "|" + card.info + "|" + doneInt;
            LogState(itemSearch);
            LogState(itemChange);
            int index = lines.FindIndex(s => s.Contains(itemSearch));
            if (index != -1)
            {
                lines[index] = lines[index].Replace(itemSearch, itemChange);
                File.WriteAllLines(path, lines);
                card.name = nameNew;
            }
        }
        public void DelCard(Card card)
        {
            bool checkDel = DeleteCard(card);
            if (checkDel == false) LogState("delCard problem");
        }
        public void CreateTask(string name, string idCard)
        {
            bool done = false;
            Task task = new Task(name, idCard);
            bool check = AddTask(task);
        }
        public void CountParticipants(Card card) // добавить в форму
        {
            int countUsers = 0;
            Desk desk;
            desk = GetDesk(card.desk_id);
            string deskString = desk.id + ".desk";
            string path = "data\\desks\\" + deskString;
            var lines = File.ReadAllLines(path).ToList();
            foreach(string userID in user_ids)
            {
                string itemSearch = "|" + userID;
                countUsers += lines.Count(line => line.Contains(itemSearch));
            }
            LogState( card.name + " users: " +  countUsers);
        }
    }
}