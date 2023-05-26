namespace TaskManager
{
    internal class Task
    {
        public string name; //имя пункта 
        public bool done; //состояние 

        public static bool GetDone(string done)//возвращает статус задачи (при неудачном прочтении или со статусом '0' возвращает false) 
        {
            switch (done)
            {
                case "0": return false; break;
                case "1": return true; break;
            }
            return false;
        }
        public Task(string name)//ручное создание 
        {
            this.name = name;
            done = false;
        }
        public Task(Task task)//клонирование 
        {
            done = task.done;
            if(task.name.Clone() as string != null) name = task.name.Clone() as string;
            else name = string.Empty;
        }
        public Task()//пустышка 
        {
            name = string.Empty;
            done = false;
        }
    }
}
