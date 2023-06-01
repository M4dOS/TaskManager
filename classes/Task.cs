namespace TaskManager
{
    public class Task
    {
        public string name; //имя пункта 
        public bool done; //состояние 
        public string card_id; //идентификатор карточки

        public static bool GetDone(string done)//возвращает статус задачи (при неудачном прочтении или со статусом '0' возвращает false) 
        {
            switch (done)
            {
                case "0": return false; break;
                case "1": return true; break;
            }
            return false;
        }
        public Task(string name, string card_id)//ручное создание 
        {
            this.name = name;
            done = false;
            this.card_id = card_id;
        }
        public Task(Task task)//клонирование 
        {
            done = task.done;
            if ((task.name.Clone() as string) != null) name = task.name.Clone() as string;
            else name = string.Empty;
            if ((task.card_id.Clone() as string) != null) card_id = task.card_id.Clone() as string;
            else card_id = string.Empty;
        }
        public Task()//пустышка 
        {
            name = string.Empty;
            done = false;
            card_id = string.Empty;
        }
    }
}
