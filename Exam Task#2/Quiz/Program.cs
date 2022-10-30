
using System.Xml.Schema;
using Quiz_Library;
using Quiz_Library.JsonFile_Save_Load;
User_Data tempData = new User_Data(new List<User>());

DirectoryInfo dir = new DirectoryInfo(Environment.CurrentDirectory);
string USER_DATA = dir.Parent.Parent.Parent.Parent + "\\Data\\Users.Json";

User_Data userData = new User_Data(new List<User>());
try
{
    userData = new User_Data(JsonSave.loadAsync(USER_DATA));
}
catch { }
DirectoryInfo dir2 = new DirectoryInfo(Environment.CurrentDirectory);
string QUIZ_DATA = dir2.Parent.Parent.Parent.Parent + "\\Data\\Quiz.Json";
Quizes quizes = new Quizes(new List<Quiz>());
try
{
    quizes = JsonSaveQuiz.loadAsync(QUIZ_DATA);
}
catch { }
ret1: if (userData.GetUsers().Count == 0)
{
    Console.WriteLine("You're the first user, create your account -> \n");
    userData.Add_User();
    await JsonSave.saveAsync(userData.GetUsers(), USER_DATA);
    goto ret1;
}
else
{
    ConsoleKeyInfo key;
    do
    {
        ret: Console.Clear();
        Console.WriteLine("\tLogin Menu");
        Console.WriteLine("L -> Log in");
        Console.WriteLine("C -> Create account");
        key = Console.ReadKey(true);
        switch (key.Key)
        {
            case ConsoleKey.L:
            loginreturn: Console.Clear();
                Console.Write("\n\tEnter login: ");
                string? temp_login = Console.ReadLine();
                if (temp_login == null || temp_login.Length <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n\tIncorrect input!");
                    Console.ReadKey(true);
                    Console.ResetColor();
                    goto loginreturn;
                }

                var Account = from acc in userData.GetUsers()
                              where acc.Name == temp_login
                              select acc;
                if (Account.Count() != 0)
                {
                    User current_User = Account.ElementAt(0);
                    int password_attempts = 0;
                Passwordreturn: Console.Clear();
                    Console.WriteLine("\n\tCurrent User Name: [" + current_User.Name + "]");
                    Console.Write("\n\tEnter password: ");
                    string? currentpassword = Console.ReadLine();
                    if (currentpassword == null || currentpassword.Length <= 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\n\tIncorrect input!");
                        Console.ReadKey(true);
                        Console.ResetColor();
                        goto Passwordreturn;
                    }

                    currentpassword = sha256.ToSHA256(currentpassword);
                    if (currentpassword == current_User.Password)
                    {
                        ConsoleKeyInfo userkeyinfo;
                        do
                        {
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\n\tCurrent user menu ["+current_User.Name+"]");
                            Console.ResetColor();
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("\n\t[Q -> Start new quiz]");
                            Console.WriteLine("\t[H -> View quiz history]");
                            Console.WriteLine("\t[V -> View Top - 20]");
                            Console.WriteLine("\t[S -> User settings]");
                            Console.WriteLine("\t[ESC -> Exit]");
                            Console.ResetColor();
                            userkeyinfo = Console.ReadKey(true);
                            switch (userkeyinfo.Key)
                            {
                                case ConsoleKey.Q:
                                    try
                                    {
                                        quizes = JsonSaveQuiz.loadAsync(QUIZ_DATA);
                                    }
                                    catch { }

                                    int avquiz = 0;
                                    for (int i = 1; i < quizes.QuizList.Count; i++)
                                    {
                                        if (quizes.QuizList[i].list.Count >= 20)
                                            avquiz++;
                                    }
                                    Console.Clear();
                                    if (quizes.QuizList.Count > 0)
                                    {
                                        if (quizes.QuizList.Count == 1)
                                        {
                                            Console.WriteLine("\n\tYou have only mixed quiz available, you can't play mixed quiz if you don't have any others");
                                            break;
                                        }
                                        else 
                                        { 
                                            indexretu: if (avquiz >= 2)
                                            {
                                                List<int> positions = new List<int>();
                                                Console.WriteLine("\n\tAvailable Quizzes");
                                                for (int i = 0; i < quizes.QuizList.Count; i++)
                                                {
                                                    if (quizes.QuizList[i].list.Count >= 20 ||
                                                        quizes.QuizList[i].Quiz_Name == "Mixed Quiz")
                                                    {
                                                        Console.WriteLine("\n\tQuiz index[" + i + "] quiz name: " + quizes.QuizList[i].Quiz_Name);
                                                        positions.Add(i);
                                                    }
                                                }
                                                Console.ForegroundColor = ConsoleColor.Yellow;
                                                Console.Write("\n\tEnter quiz index, in which u wanna play: ");
                                                try
                                                {
                                                    int index = Convert.ToInt32(Console.ReadLine());
                                                    if (index < 0 || index > quizes.QuizList.Count - 1)
                                                    {
                                                        Console.ForegroundColor = ConsoleColor.Red;
                                                        Console.WriteLine("\n\tIncorrect input!");
                                                        Console.ResetColor();
                                                        Console.Clear();
                                                        goto indexretu;
                                                    }

                                                    int flag = 0;
                                                    for (int i = 0; i < positions.Count; i++)
                                                    {
                                                        if (index == positions[i])
                                                        {
                                                            flag = 1;
                                                            break;
                                                        }
                                                    }

                                                    if (flag == 0)
                                                    {
                                                        Console.ForegroundColor = ConsoleColor.Red;
                                                        Console.WriteLine("\n\tIncorrect input!");
                                                        Console.ResetColor();
                                                        Console.Clear();
                                                        goto indexretu;
                                                    }
                                                    if (index == 0)
                                                    {
                                                        Play_Quiz.PlaymixedQuiz(quizes, current_User, quizes.QuizList[0]);
                                                        await JsonSaveQuiz.saveAsync(quizes, QUIZ_DATA);
                                                        quizes = JsonSaveQuiz.loadAsync(QUIZ_DATA);
                                                    }
                                                    else
                                                    {
                                                        Play_Quiz.Play(quizes.QuizList[index], current_User);
                                                        await JsonSaveQuiz.saveAsync(quizes, QUIZ_DATA);
                                                        quizes = JsonSaveQuiz.loadAsync(QUIZ_DATA);
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.ForegroundColor = ConsoleColor.Red;
                                                    Console.WriteLine("\n\tIncorrect input!");
                                                    Console.ResetColor();
                                                    Console.Clear();
                                                    goto indexretu;
                                                }
                                                Console.ResetColor();
                                            }
                                            else
                                            {
                                                List<int> positions = new List<int>();
                                                Console.WriteLine("\n\tAvailable Quizzes");
                                                for (int i = 0; i < quizes.QuizList.Count; i++)
                                                {
                                                    if (quizes.QuizList[i].list.Count >= 20)
                                                    {
                                                        Console.WriteLine("\n\tQuiz index[" + i + "] quiz name: " + quizes.QuizList[i].Quiz_Name);
                                                        positions.Add(i);
                                                    }
                                                }
                                                Console.ForegroundColor = ConsoleColor.Yellow;
                                                Console.Write("\n\tEnter quiz index, in which u wanna play: ");
                                                try
                                                {
                                                    int index = Convert.ToInt32(Console.ReadLine());
                                                    if (index < 1 || index > quizes.QuizList.Count - 1)
                                                    {
                                                        Console.ForegroundColor = ConsoleColor.Red;
                                                        Console.WriteLine("\n\tIncorrect input!");
                                                        Console.ResetColor();
                                                        Console.Clear();
                                                        goto indexretu;
                                                    }

                                                    int flag = 0;
                                                    for (int i = 0; i < positions.Count; i++)
                                                    {
                                                        if (positions[i] == index)
                                                        {
                                                            flag = 1;
                                                            break;
                                                        }
                                                    }

                                                    if (flag == 0)
                                                    {
                                                        Console.ForegroundColor = ConsoleColor.Red;
                                                        Console.WriteLine("\n\tIncorrect input!");
                                                        Console.ResetColor();
                                                        Console.Clear();
                                                        goto indexretu;
                                                    }
                                                    Play_Quiz.Play(quizes.QuizList[index], current_User);
                                                    await JsonSaveQuiz.saveAsync(quizes, QUIZ_DATA);
                                                    quizes = JsonSaveQuiz.loadAsync(QUIZ_DATA);
                                                    
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.ForegroundColor = ConsoleColor.Red;
                                                    Console.WriteLine("\n\tIncorrect input!");
                                                    Console.ResetColor();
                                                    Console.Clear();
                                                    goto indexretu;
                                                }
                                                Console.ResetColor();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("\n\tU don't have any quizzes");
                                    }
                                    Console.ReadKey(true);
                                    break;
                                case ConsoleKey.H:
                                    Console.Clear();
                                    int m = 0;
                                    for (int i = 0; i < quizes.QuizList.Count; i++)
                                    {
                                        for (int d = 0; d < quizes.QuizList[i].Top20.Top20List.Count; d++)
                                        {
                                            if (current_User.Name == quizes.QuizList[i].Top20.Top20List[d].Name)
                                            {
                                                m = 1;
                                                Console.WriteLine("\n\tQuiz -> "+ quizes.QuizList[i].Quiz_Name);
                                                var place = from p in quizes.QuizList[i].Top20.Top20List
                                                    orderby p.Points descending
                                                    select p;
                                                for (int f = 0; f < place.Count(); f++)
                                                {
                                                    if (place.ElementAt(f).Name == current_User.Name)
                                                    {
                                                        Console.WriteLine("\n\tYour Place in this quiz(Show place based on your best score) - [" + (f + 1) + "]");
                                                        Console.WriteLine("\n\tYour Best Score - " + place.ElementAt(f).Points + " point(s)");
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if(m == 0)
                                        Console.WriteLine("\n\tU didn't participate in any quizzes");
                                    Console.ReadKey(true);
                                    break;
                                case ConsoleKey.V://Top 20
                                    Console.Clear();
                                    int avquiza = 0;
                                    List<Quiz> quizList = new List<Quiz>();
                                    for (int i = 0; i < quizes.QuizList.Count; i++)
                                    {
                                        if (quizes.QuizList[i].list.Count >= 20 || quizes.QuizList[i].Quiz_Name == "Mixed Quiz")
                                        {
                                            avquiza++;
                                            quizList.Add(quizes.QuizList[i]);
                                        }
                                    }
                                    
                                    if (avquiza > 0)
                                    {
                                        Top20: Console.Write("\n\tEnter Quiz Name to see it's top 20 -> ");
                                        string? quiznametop20 = Console.ReadLine();
                                        if (quiznametop20 == null || quiznametop20.Length <= 0)
                                        {
                                            Console.ForegroundColor = ConsoleColor.Yellow;
                                            Console.WriteLine("\n\tIncorrect input!");
                                            Console.ResetColor();
                                            Console.ReadKey(true);
                                            Console.Clear();
                                            goto Top20;

                                        }

                                        int flag = 0;
                                        foreach (var VARIABLE in quizList)
                                        {
                                            if (VARIABLE.Quiz_Name == quiznametop20)
                                            {
                                                flag = 1;
                                                if (VARIABLE.Top20.Top20List.Count > 0)
                                                {
                                                    var places = from pls in VARIABLE.Top20.Top20List
                                                        orderby pls.Points descending 
                                                            select pls;
                                                    int placex = 1;
                                                    foreach (var place in places)
                                                    {
                                                        Console.WriteLine("\n\tPlace ["+placex+"] | User: " + place.Name + " | Score: " + place.Points + " points | Time: "+ place.Time + " sec");
                                                        if (placex == 20)
                                                            break;
                                                        placex++;
                                                        
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine("\n\tNobody has participated in this quiz");
                                                }
                                            }
                                        }

                                        if (flag == 0)
                                        {
                                            Console.WriteLine("\n\tQuiz wasn't found or this quiz isn't available");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("\n\tU don't have any available quizzes");
                                    }
                                    Console.ReadKey(true);
                                    break;
                                case ConsoleKey.S:
                                    password_attempts = 0;
                                Settingsreturn: Console.Clear();
                                    Console.Write("\n\tEnter account password to manage settings: ");
                                    
                                    string? settingspassworcd = Console.ReadLine();
                                    if (settingspassworcd == null || settingspassworcd.Length <= 0)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        Console.WriteLine("\n\tIncorrect input!");
                                        Console.ReadKey(true);
                                        Console.ResetColor();
                                        goto Settingsreturn;
                                    }

                                    string decoded_password = settingspassworcd;
                                    settingspassworcd = sha256.ToSHA256(settingspassworcd);
                                    if (settingspassworcd == current_User.Password)
                                    {
                                        Console.Clear();
                                        Console.WriteLine("\n\tCurrent User: ["+ current_User.Name + "]");
                                        Console.WriteLine("\n\tPassword: [" + decoded_password + "]");
                                        Console.WriteLine("\n\tBirth date: ["+ current_User.BirthDate + "]");
                                        Console.WriteLine("\n\tPress 'E' to edit your data.");
                                        ConsoleKeyInfo settingskey = Console.ReadKey(true);
                                        if (settingskey.Key == ConsoleKey.E)
                                        {
                                            editreturn: Console.Clear();
                                            Console.WriteLine("\n\tSelect property to change -> ");
                                            Console.WriteLine("\n\tP -> Password");
                                            Console.WriteLine("\n\tD -> Birth date");
                                            Console.WriteLine("\n\tESC -> Exit");
                                            ConsoleKeyInfo editkey = Console.ReadKey(true);
                                            if (editkey.Key != ConsoleKey.P && editkey.Key != ConsoleKey.D &&
                                                editkey.Key != ConsoleKey.Escape)
                                                goto editreturn;
                                            switch (editkey.Key)
                                            {
                                                case ConsoleKey.P:
                                                    Console.Clear();
                                                    ret2: Console.Write("\n\tEnter new password: ");
                                                    string? password = Console.ReadLine();
                                                    if (password == null || password.Length < 4)
                                                    {
                                                        Console.ForegroundColor = ConsoleColor.Red;
                                                        Console.WriteLine("\n\tIncorrect input!");
                                                        Console.ReadKey(true);
                                                        Console.Clear();
                                                        Console.ResetColor();
                                                        goto ret2;
                                                    }

                                                    
                                                    userData.Update_Settings(ref current_User, current_User.Name, sha256.ToSHA256(password), current_User.BirthDate);
                                                    await JsonSave.saveAsync(userData.GetUsers(), USER_DATA);
                                                    break;
                                                case ConsoleKey.D:
                                                    Console.Clear();
                                                ret3: Console.Write("\n\tEnter your Birth date(yy//mm//dd)");
                                                    string? date = Console.ReadLine();
                                                    if (date == null || date.Split().Length != 3)
                                                    {
                                                        Console.ForegroundColor = ConsoleColor.Red;
                                                        Console.WriteLine("\n\tIncorrect input!");
                                                        Console.ReadKey(true);
                                                        Console.Clear();
                                                        Console.ResetColor();
                                                        goto ret3;
                                                    }
                                                    DateOnly bdd = new DateOnly();
                                                    try
                                                    {
                                                        DateOnly bd = new DateOnly(Convert.ToInt32(date.Split()[0]), Convert.ToInt32(date.Split()[1]), Convert.ToInt32(date.Split()[2]));
                                                        if (bd.Year >= 2021)
                                                        {
                                                            Console.ForegroundColor = ConsoleColor.Red;
                                                            Console.WriteLine("\n\tIncorrect input!");
                                                            Console.ReadKey(true);
                                                            Console.Clear();
                                                            Console.ResetColor();
                                                            goto ret3;
                                                        }
                                                        bdd = bd;
                                                    }
                                                    catch (Exception e)
                                                    {
                                                        Console.ForegroundColor = ConsoleColor.Red;
                                                        Console.WriteLine("\n\tIncorrect input!");
                                                        Console.ReadKey(true);
                                                        Console.Clear();
                                                        Console.ResetColor();
                                                        goto ret3;
                                                    }
                                                    userData.Update_Settings(ref current_User, current_User.Name, current_User.Password, bdd.ToShortDateString());
                                                    await JsonSave.saveAsync(userData.GetUsers(), USER_DATA);
                                                    break;
                                            }
                                            Console.ReadKey(true);
                                        }
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Yellow;
                                        password_attempts++;
                                        if (password_attempts != 3)
                                        {
                                            Console.WriteLine("\n\tIncorrect password\n\n\t[Attempts left - " + (3 - password_attempts) + "]");
                                        }
                                        else
                                        {
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine("\n\tExhausted amount of password attempts! Try again later.");
                                            Console.ResetColor();
                                        }
                                        Console.ReadKey(true);

                                        Console.ResetColor();
                                        if (password_attempts != 3)
                                            goto Settingsreturn;
                                    }
                                    break;
                            }
                        } while (userkeyinfo.Key != ConsoleKey.Escape);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        password_attempts++;
                        if (password_attempts != 3)
                        {
                            Console.WriteLine("\n\tIncorrect password\n\n\t[Attempts left - " + (3 - password_attempts) + "]");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\n\tExhausted amount of password attempts! Try again later.");
                            Console.ResetColor();
                        }
                        Console.ReadKey(true);
                    
                        Console.ResetColor();
                        if (password_attempts != 3)
                            goto Passwordreturn;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n\tAccount wasn't found :(");
                    Console.ResetColor();
                    Console.ReadKey(true);
                }
                
                break;
            case ConsoleKey.C:
                Console.Clear();
                Console.WriteLine("\n\tCreate new account -> \n");
                userData.Add_User();
                await JsonSave.saveAsync(userData.GetUsers(), USER_DATA);
                Console.ReadKey(true);
                break;
        }

    } while (key.Key != ConsoleKey.Escape);
    
}
