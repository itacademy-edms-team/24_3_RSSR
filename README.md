# 📰 RSS Reader - Документация проекта

## 🚀 Обзор проекта
**RSS Reader** - это веб-приложение для агрегации новостей из RSS-лент с возможностью:
- Просмотра последних новостей в единой ленте
- Добавления и хранения любимых RSS-источников
- Поиска по содержимому новостей

## 🛠 Технологический стек
- **Backend**: ASP.NET Core 8
- **Frontend**: Razor Pages + Bootstrap 
- **Парсинг RSS**: `System.ServiceModel.Syndication`
- **Хранение данных**: JSON-файл (`feeds.json`)

## 📦 Установка и запуск
1. Клонировать репозиторий:
   ```bash
   git clone https://github.com/ваш-username/RSS-Reader.git
   cd RSS-Reader
   ```
2. Восстановить зависимости:
   ```bash
   dotnet restore
   ```
3. Запустить приложение:
   ```bash
   dotnet run
   ```
4. Открыть в браузере:
   ```
   http://localhost:5229
   ```

## 🔧 Функционал
### Основные возможности
✅ Просмотр новостей в карточках  
✅ Добавление новых RSS-источников  
✅ Автоматическое сохранение списка лент   
✅ Очистка текста от HTML-разметки  

### Примеры рабочих RSS-лент
```
https://habr.com/ru/rss/news/
https://xakep.ru/feed/
http://feeds.bbci.co.uk/news/technology/rss.xml
```

## 🏗 Структура проекта
```
RSSReader/
├── Pages/               # Razor-страницы
│   ├── Index.cshtml     # Главная страница
├── Services/           # Бизнес-логика
│   ├── RssParser.cs    # Парсер RSS
│   └── FeedStorage.cs  # Работа с данными
├── Models/             # Модели данных
│   └── FeedItem.cs     # Модель новости
└── wwwroot/            # Статические файлы
```

💡 **Совет**: Чтобы найти RSS-ленту на любом сайте, попробуйте добавить `/feed` или `/rss` к основному URL.
