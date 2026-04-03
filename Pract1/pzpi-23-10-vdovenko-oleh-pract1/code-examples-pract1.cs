/*
Запит до ШІ:
«Напиши приклад реалізації поведінкового патерна проєктування Command на мові C#. 
Приклад має бути простим і зрозумілим, наприклад, система керування освітленням (розумна лампа). 
Наведи код базового інтерфейсу команди, класу отримувача (лампи), конкретної команди 
для увімкнення світла та приклад їх використання у клієнтському коді програми. Зроби все одним файлом.»
*/

using System;

public interface ICommand
{
    void Execute();
}

public class Light
{
    public void TurnOn()
    {
        Console.WriteLine("Світло увімкнено");
    }
}

public class TurnOnLightCommand : ICommand
{
    private Light light;

    public TurnOnLightCommand(Light light)
    {
        this.light = light;
    }

    public void Execute()
    {
        light.TurnOn();
    }
}

class Program
{
    static void Main()
    {
        // Створюємо отримувача
        Light light = new Light();
        
        // Створюємо команду і зв'язуємо її з отримувачем
        ICommand command = new TurnOnLightCommand(light);
        
        // Ініціатор викликає команду
        command.Execute();
    }
}
