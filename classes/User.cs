using System.Text;

namespace TaskManager
{
    public class User
    {
        public string login; //логин 
        public string id; //идентификатор пользователя 
        public string password; //пароль 
        public string name; //имя 
        public List<string> owner; //доски, которыми он владеет 
        public List<string> admin; //доски, с которыми можно взаимодействовать 
        public List<string> guest; //доски, которые можно только смотреть 

        public string IDCreator(DataBase db)//создание идентификатора
        {
            int size = 4;
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
            } while (db.user_ids.Contains(id));
            db.user_ids.Add(id);
            return id;
        }
        public User(string login, string password, string name, DataBase db)//ручное создание
        {
            this.login = login;
            this.password = password;
            this.name = name;
            id = IDCreator(db);
            owner = new();
            admin = new();
            guest = new();
        }
        public User(string id, string login, string password, string name, DataBase db)//ручное создание (вкл. идентификатор) 
        {
            this.login = login;
            this.password = password;
            this.name = name;
            this.id = id;
            db.user_ids.Add(id);
            owner = new();
            admin = new();
            guest = new();
        }
        public User(User user, DataBase db, bool unic_id)//клонирование
        {
            if ((user.login.Clone() as string) != null) login = user.login.Clone() as string;
            else login = string.Empty;
            if ((user.password.Clone() as string) != null) password = user.password.Clone() as string;
            else password = string.Empty;
            if ((user.name.Clone() as string) != null) name = user.name.Clone() as string;
            else name = string.Empty;
            if (unic_id) id = IDCreator(db);
            else id = user.id;
            owner = user.owner.ToList();
            admin = user.admin.ToList();
            guest = user.guest.ToList();
        }
        public User(DataBase db, bool id_need)//пустышка
        {
            login = string.Empty;
            password = string.Empty;
            name = string.Empty;
            if (id_need) id = IDCreator(db);
            else id = string.Empty;
            owner = new();
            admin = new();
            guest = new();
        }
    }
}
