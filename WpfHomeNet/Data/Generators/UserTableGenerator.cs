namespace WpfHomeNet.Data.Generators
{
    public class UsersTableGenerator : TableGenerator
    {
        public UsersTableGenerator(): base( "users")
            
        {               
            { 
                 AddColumn("Id").Integer().PrimaryKey().AutoIncrement().Build();
                    
                 AddColumn("FirstName").Varchar(255).NotNull() .Build();    
                    
                 AddColumn("LastName") .Varchar(255).Build(); // NULL разрешён
                    
                 AddColumn("PhoneNumber").Varchar(255).Build();  // NULL разрешён   
                   
                 AddColumn("Email").Varchar(255).NotNull().Unique().Build();
               
                 AddColumn("Password").Varchar(255).NotNull().Build();               
                    
                 AddColumn("CreatedAt").NotNull().CreatedAt().Build();
                  
            };
        }
    }



}
