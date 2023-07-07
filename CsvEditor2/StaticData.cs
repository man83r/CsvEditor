using System;


namespace CsvEditor
{
    public static class StaticData
    {
        public static String DataBuffer = String.Empty;

        //поля для поиска и замены текста, а так же флаг
        public static String SourceText = String.Empty;

        public static String TextForReplace = String.Empty;

        public static bool replaceMarker = false;

        //поле для отлавливания строки выделенной (над которой мышка) ячейки
        public static int numberRow;

        public static int numberCol;

        //сохраняем путь к cfg при запуске программы
        public static string CSV_CONFIG_PATH;
    }
}
