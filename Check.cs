namespace TaskManager
{
    internal class Check
    {
        public string id_card; //привязка к карточке 
        public List<Task> tasks; //"пункты" чек-листа 
        public bool done; //состояние сделанности 

        public static bool GetDone(string done)//возвращает статус задачи (при неудачном прочтении или со статусом '0' возвращает false) 
        {
            switch (done)
            {
                case "0": return false; break;
                case "1": return true; break;
            }
            return false;
        }
        public Check(string id_card, bool done)//ручное создание 
        {
            this.id_card = id_card;
            tasks = new();
            this.done = done;
        }
        public Check(Check check)//клонирование 
        {
            done = check.done;
            tasks = check.tasks.ToList();
            if (check.id_card.Clone() as string != null) id_card = check.id_card.Clone() as string;
            else id_card = string.Empty;
        }
        public Check()//пустышка 
        {
            id_card = string.Empty;
            tasks = new();
            done = false;
        }
    }
}
