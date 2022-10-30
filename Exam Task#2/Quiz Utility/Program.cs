using Quiz_Library;
using Quiz_Library.JsonFile_Save_Load;

User_Data tempData = new User_Data(new List<User>());

DirectoryInfo dir = new DirectoryInfo(Environment.CurrentDirectory);
string QUIZ_DATA = dir.Parent.Parent.Parent.Parent + "\\Data\\Quiz.Json";
Quizes quizes = new Quizes(new List<Quiz>());
try
{
    quizes = JsonSaveQuiz.loadAsync(QUIZ_DATA);
}
catch {}
DirectoryInfo dir2 = new DirectoryInfo(Environment.CurrentDirectory);
string USER_DATA = dir.Parent.Parent.Parent.Parent + "\\Data\\Users.Json";

User_Data userData = new User_Data(new List<User>());
try
{
    userData = new User_Data(JsonSave.loadAsync(USER_DATA));
}
catch { }

ret1: if (userData.GetUsers().Count == 0)
{
    Console.WriteLine("You're the first user, create your account -> \n");
    userData.Add_User();
    JsonSave.saveAsync(userData.GetUsers(), USER_DATA);
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
                            Console.WriteLine("\n\tCurrent user menu [" + current_User.Name + "]");
                            Console.ResetColor();
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("\n\t[Q -> Create Quiz]");
                            Console.WriteLine("\t[E -> Edit Quiz]");
                            Console.WriteLine("\t[X -> Edit Question]");
                            Console.WriteLine("\t[A -> Add Question]");
                            Console.WriteLine("\t[S -> View all Quizzes]");
                            Console.WriteLine("\t[V -> View all questions in quiz]");
                            Console.WriteLine("\t[ESC -> Exit]");
                            Console.ResetColor();
                            userkeyinfo = Console.ReadKey(true);
                            switch (userkeyinfo.Key)
                            {
                                case ConsoleKey.Q:
                                    Console.Clear();
                                    quizes.Add_Quiz();
                                    await JsonSaveQuiz.saveAsync(quizes, QUIZ_DATA);
                                    quizes = JsonSaveQuiz.loadAsync(QUIZ_DATA);
                                    Console.ReadKey(true);
                                    break;
                                case ConsoleKey.E:
                                    Console.Clear();
                                    if (quizes.QuizList.Count > 0)
                                    {
                                        quizes.EditQuizName();
                                        await JsonSaveQuiz.saveAsync(quizes, QUIZ_DATA);
                                        quizes = JsonSaveQuiz.loadAsync(QUIZ_DATA);
                                    }
                                    else
                                    {
                                        Console.WriteLine("You have no quizzes in data base");
                                    }

                                    Console.ReadKey(true);
                                    break;
                                case ConsoleKey.X:
                                    Console.Clear();
                                    if (quizes.QuizList.Count > 0)
                                    {
                                        quizes.EditQuestion(quizes);
                                        await JsonSaveQuiz.saveAsync(quizes, QUIZ_DATA);
                                        quizes = JsonSaveQuiz.loadAsync(QUIZ_DATA);
                                    }
                                    else
                                    {
                                        Console.WriteLine("You have no quizzes in data base");
                                    }

                                    Console.ReadKey(true);
                                    break;
                                case ConsoleKey.A:
                                    Console.Clear();
                                    if (quizes.QuizList.Count > 0)
                                    {
                                    ret2: Console.Write("\n\tEnter Quiz Name: ");
                                        int p = 0;
                                        string? qName = Console.ReadLine();
                                        if (qName == null || qName.Length <= 0)
                                        {
                                            Console.ForegroundColor = ConsoleColor.Yellow;
                                            Console.WriteLine("\n\tIncorrect input!");
                                            Console.ResetColor();
                                            Console.ReadKey();
                                            Console.Clear();
                                            goto ret2;
                                        }

                                        if (qName == "Mixed Quiz")
                                        {
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine("\n\tU can't add questions to mixed quiz!");
                                            Console.ResetColor();
                                            Console.ReadKey();
                                            Console.Clear();
                                            goto ret2;
                                        }
                                        int f = 0;
                                        for (int i = 0; i < quizes.QuizList.Count; i++)
                                        {
                                            if (quizes.QuizList[i].Quiz_Name == qName)
                                            {
                                                p = 1;
                                                quizes.QuizList[i].Add_quiz_question(quizes);
                                                await JsonSaveQuiz.saveAsync(quizes, QUIZ_DATA);
                                                quizes = JsonSaveQuiz.loadAsync(QUIZ_DATA);
                                                Console.WriteLine("\n\tQuestion was successfully added");
                                                break;
                                            }
                                        }
                                        if (p == 0)
                                            Console.WriteLine("\n\tQuiz wasn't found :(");
                                        Console.ReadKey(true);
                                        await JsonSaveQuiz.saveAsync(quizes, QUIZ_DATA);
                                        quizes = JsonSaveQuiz.loadAsync(QUIZ_DATA);
                                    }
                                    else
                                    {
                                        Console.WriteLine("You have no quizzes in data base");
                                    }

                                    Console.ReadKey(true);
                                    break;
                                case ConsoleKey.S:
                                    Console.Clear();
                                    if (quizes.QuizList.Count == 1 && quizes.QuizList[0].Quiz_Name == "Mixed Quiz")
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("\n\tYou have no quizzes except mixed quiz, You can't view mixed quiz");
                                        Console.ResetColor();
                                    }
                                    if (quizes.QuizList.Count > 0)
                                    {
                                        for (int i = 0; i < quizes.QuizList.Count; i++)
                                        {
                                            if (quizes.QuizList[i].Quiz_Name != "Mixed Quiz")
                                                Console.WriteLine("\n\tQuiz -> [" + quizes.QuizList[i].Quiz_Name + "]");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("You have no quizzes in data base");
                                    }

                                    Console.ReadKey(true);
                                    break;
                                case ConsoleKey.V:
                                    {
                                        if (quizes.QuizList.Count > 0)
                                        {
                                        ret2: Console.Write("\n\tEnter Quiz Name: ");
                                            int p = 0;
                                            string? qName = Console.ReadLine();
                                            if (qName == null || qName.Length <= 0)
                                            {
                                                Console.ForegroundColor = ConsoleColor.Yellow;
                                                Console.WriteLine("\n\tIncorrect input!");
                                                Console.ResetColor();
                                                Console.ReadKey();
                                                Console.Clear();
                                                goto ret2;
                                            }
                                            if (qName == "Mixed Quiz")
                                            {
                                                Console.ForegroundColor = ConsoleColor.Red;
                                                Console.WriteLine("\n\tU can't see questions in mixed quiz!");
                                                Console.ResetColor();
                                                Console.ReadKey();
                                                Console.Clear();
                                                goto ret2;
                                            }
                                            int f = 0;
                                            for (int i = 0; i < quizes.QuizList.Count; i++)
                                            {
                                                if (quizes.QuizList[i].Quiz_Name == qName)
                                                {
                                                    p = 1;
                                                    foreach (var VARIABLE in quizes.QuizList[i].list)
                                                    {
                                                        Console.WriteLine("\n\tQuestion -> [" + VARIABLE.Question_content + "]");
                                                        f = 1;
                                                    }
                                                    break;
                                                }
                                            }
                                            if (p != 0 && f == 0)
                                                Console.WriteLine("\n\tThis quiz has no questions yet");
                                            if (p == 0)
                                                Console.WriteLine("\n\tQuiz wasn't found :(");
                                            Console.ReadKey(true);
                                        }
                                        else
                                        {
                                            Console.WriteLine("\n\tYou have no quizzes in data base");
                                        }
                                        Console.ReadKey(true);
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
                JsonSave.saveAsync(userData.GetUsers(), USER_DATA);
                Console.ReadKey(true);
                break;
        }

    } while (key.Key != ConsoleKey.Escape);

}