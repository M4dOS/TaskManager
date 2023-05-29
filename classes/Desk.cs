using System.Text;

namespace TaskManager
{
    public enum Type { Error = -1, Private, Public }
    public class Desk
    {
        public string name; //название доски 
        public string id; //идентификатор доски 
        public User owner; //создатель доски 
        public Type type; //тип доски (личная/публичная) 
        public List<Card> cards; //"карточки" 


        string IDCreator(DataBase db)//создание идентификатора
        {
            int size = 8;
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
            } while (db.desk_ids.Contains(id));
            db.desk_ids.Add(id);
            return id;
        }

        public static Type GetType(int type_id)//возвращает тип доски (при неудаче возвращает Type.Error) 
        {
            switch (type_id)
            {
                case 0: return Type.Private;
                case 1: return Type.Public;
                default: return Type.Error;
            }
        }
        public Desk(string name, User user, Type type, DataBase db)//ручное создание 
        {
            this.name = name;
            owner = user;
            id = IDCreator(db);
            this.type = type;
            cards = new();
        }
        public Desk(string id, string name, User user, Type type, DataBase db)//ручное создание (вкл. идентификатор) 
        {
            this.name = name;
            owner = user;
            this.id = id;
            db.desk_ids.Add(id);
            this.type = type;
            cards = new();
        }
        public Desk(Desk desk, DataBase db, bool unic_id)//клонирование 
        {
            if ((desk.name.Clone() as string) != null) name = desk.name.Clone() as string;
            else name = string.Empty;
            /*owner = desk.owner;*/
            if (unic_id) id = IDCreator(db);
            else id = desk.id;
            type = desk.type;
            cards = desk.cards.ToList();
        }
        public Desk(DataBase db, bool id_need)//пустышка 
        {
            name = string.Empty;
            if (id_need) { id = IDCreator(db); owner = new(db, false); }
            else { id = string.Empty; owner = new(db, false); }
            type = Type.Private;
            cards = new();
        }
    }
}
