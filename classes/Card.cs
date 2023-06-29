using System.Text;

namespace TaskManager
{
    public class Card
    {
        /*public List<string> pic_paths; //для ссылок на фотки (под сомнением) */

        public string name; //имя карточки (задачи) 
        public string info; //дополнительная информация 
        public Check checkList; //"чек-лист" 
        public string id; //идентификатор задачи 
        public string desk_id; //идентификатор привязанной доски 
        public bool done; //состояние задачи 

        public int xLocation;

        string IDCreator(DataBase db)//создание идентификатора 
        {
            int size = 12;
            string id = string.Empty;
            do
            {
                StringBuilder builder = new StringBuilder();
                Enumerable
                   .Range(65, 26)
                    .Select(e => ((char)e).ToString())
                    .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                    .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                    .OrderBy(e => Guid.NewGuid())
                    .Take(size)
                    .ToList().ForEach(e => builder.Append(e));
                id = builder.ToString();
            } while (db.card_ids.Contains(id));
            db.card_ids.Add(id);
            return id;
        }

        public static bool GetDone(string done)//возвращает статус задачи (при неудачном прочтении или со статусом '0' возвращает false) 
        {
            switch (done)
            {
                case "0": return false; break;
                case "1": return true; break;
            }
            return false;
        }
        public Card(string desk_id, string name, bool done, string info, DataBase db)//ручное создание 
        {
            this.name = name;
            this.desk_id = desk_id;
            this.info = info;
            checkList = new();
            id = IDCreator(db);
            this.done = done;
        }
        public Card(string id, string desk_id, string name, bool done, string info, DataBase db)//ручное создание (вкл. идентификатор) 
        {
            this.name = name;
            this.desk_id = desk_id;
            this.info = info;
            checkList = new();
            this.id = id;
            db.card_ids.Add(id);
            this.done = done;
        }
        public Card(Card card, DataBase db, bool unic_id)//клонирование 
        {
            if ((card.name.Clone() as string) != null) name = card.name.Clone() as string;
            else name = string.Empty;
            if ((card.info.Clone() as string) != null) info = card.info.Clone() as string;
            else info = string.Empty;
            checkList = new(card.checkList);
            if (unic_id) id = IDCreator(db);
            else id = card.id;
            done = false;
            desk_id = card.desk_id;
        }
        public Card(DataBase db, bool id_need)//пустышка 
        {
            name = string.Empty;
            info = string.Empty;
            desk_id = string.Empty;
            if (id_need) id = IDCreator(db);
            else id = string.Empty;
            done = false;
            checkList = new();
        }
    }
}
