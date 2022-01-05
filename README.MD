Точка входа — GameController.cs. Чтобы проект запустился надо создать на сцене пустой объект и добавить туда  GameController, и указать ему путь до ScriptableObject Data (по умолчанию — Assets/Resources/Data)

В GameInit – Инициализация основных классов и добавление контроллеров, которые выполняются в цикле в GameController

Информация для объектов берётся из ScriptableObject, названия к которым прописываются в   ScriptableObject Data по пути Assets/Resources/Data.

Чтобы выполнить метод в Start, Update и тд надо реализовать соответствующий интерфейс (IInitialization, IExecute и тд)