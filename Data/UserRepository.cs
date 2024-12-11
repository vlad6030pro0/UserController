using System;
using userproj.Models;

namespace userproj.Data;

public class UserRepository
{
    public static List<User> Users = new List<User>();

    public static void AddUser(User user){
        Users.Add(user);
    }

    public static void ActualUsers(){
        
    }

    public static User GetUser(string username){
        ActualUsers();
        var user = Users.FirstOrDefault(x => x.UserName == username);
        // if(BCrypt.Net.BCrypt.Verify(user.Password, password)){

        // }

        return user;
    }
}
