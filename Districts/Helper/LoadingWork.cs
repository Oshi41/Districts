﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Districts.Comparers;
using Districts.JsonClasses;
using Districts.JsonClasses.Base;
using Districts.JsonClasses.Manage;
using Districts.Parser;
using Districts.Settings;
using Newtonsoft.Json;

namespace Districts.Helper
{
    public static class LoadingWork
    {
        private static readonly IParser _parser;
        static LoadingWork()
        {
            _parser = new Parser.Parser();
        }

        /// <summary>
        ///     Загружаю все файлы из папки и сериализую их из json формата
        /// </summary>
        /// <typeparam name="T">Тип элементов</typeparam>
        /// <param name="folder">Путь к папке</param>
        /// <returns></returns>
        private static Dictionary<string, T> LoadSmth<T>(string folder)
        {
            var result = new Dictionary<string, T>();

            if (!Directory.Exists(folder))
            {
                Tracer.WriteError(new FileNotFoundException("Не найдена папка " + folder));
                return new Dictionary<string, T>();
            }

            var fileNames = Directory.GetFiles(folder);
            foreach (var fileName in fileNames)
            {
                var fileContent = File.ReadAllText(fileName);
                try
                {
                    var temp = JsonConvert.DeserializeObject<T>(fileContent);
                    result.Add(Path.GetFullPath(fileName), temp);
                }
                catch (Exception e)
                {
                    Tracer.WriteError(e);
                }
            }

            return result;
        }

        /// <summary>
        ///     Загружаю только ункуальные значния
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="folder"></param>
        /// <returns></returns>
        private static Dictionary<string, List<T>> LoadDistinct<T>(string folder)
            where T : BaseFindableObject
        {
            // загрузл все объекты
            var loaded = LoadSmth<List<T>>(folder);
            // сохранил их в большой список
            var allHomes = loaded.SelectMany(x => x.Value).ToList();
            // выбрал только уникальные
            allHomes = allHomes.Distinct(new BaseFindableObjectComparer()).OfType<T>().ToList();
            // вернул группированные по улице (имени файла)
            return allHomes.GroupBy(x => x.Street)
                .ToDictionary(x => x.Key, x => x.GetEnumerator().ToIEnumerable().ToList());
        }


        public static Dictionary<string, List<Building>> LoadSortedHomes()
        {
            var result = LoadDistinct<Building>(ApplicationSettings.ReadOrCreate().BuildingPath);
            foreach (var keyValuePair in result) keyValuePair.Value.Sort(new HouseNumberComparer());

            return result;
        }

        public static Dictionary<string, List<ForbiddenElement>> LoadRules()
        {
            return LoadDistinct<ForbiddenElement>(ApplicationSettings.ReadOrCreate().RestrictionsPath);
        }

        public static Dictionary<string, List<HomeInfo>> LoadCodes()
        {
            return LoadDistinct<HomeInfo>(ApplicationSettings.ReadOrCreate().HomeInfoPath);
        }

        public static Dictionary<string, Card> LoadCards()
        {
            var temp = LoadSmth<Card>(ApplicationSettings.ReadOrCreate().CardsPath);
            // сразу сортирую
            temp = temp.OrderBy(x => x.Value.Number)
                .ToDictionary(x => x.Key, x => x.Value);
            return temp;
        }

        public static Dictionary<string, CardManagement> LoadManageElements()
        {
            var result = LoadSmth<CardManagement>(ApplicationSettings.ReadOrCreate().ManageRecordsPath);
            // сразу сортирую
            result = result.OrderBy(x => x.Value.Number)
                .ToDictionary(x => x.Key, x => x.Value);
            return result;
        }
    }
}