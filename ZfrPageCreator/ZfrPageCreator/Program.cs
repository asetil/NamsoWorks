using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ZfrPageCreator
{
    internal class Program
    {
        private static string mainPath = "C:\\Projects\\ZaferParti\\";
        private static string updateFields = "";
        private static string entityFields = "";
        private static string dtoFields = "";
        private static string webFields = "";

        static void Main(string[] args)
        {
            try
            {
                //args = (new List<string>() { "OgrenimDurumu", "[string:Adi]" }).ToArray();
                if (args == null || args.Length != 2)
                {
                    Console.WriteLine("Lutfen islem parametrelerini [Entity] [Fields] formatinda ekleyin. Ornek => Person [string:Adi;int:Age;decimal:Salary]");
                    return;
                }

                var entityName = args[0];
                var fields = args[1].Replace("[", "").Replace("]", "").Split(";");
                updateFields = GetFields(fields, "update");
                entityFields = GetFields(fields, "entity");
                dtoFields = GetFields(fields, "dto");
                webFields = GetFields(fields, "web-fields");

                CreateFile("ZFR.BAL\\Manager\\Interfaces\\I" + entityName + "Manager.cs", "InterfaceManager.txt", entityName);
                CreateFile("ZFR.BAL\\Manager\\" + entityName + "Manager.cs", "Manager.txt", entityName);
                CreateFile("ZFR.BAL\\Model\\Entities\\" + entityName + ".cs", "Entity.txt", entityName);
                CreateFile("ZFR.BAL\\Model\\EntityConfiguration\\" + entityName + "Configuration.cs", "EntityConfiguration.txt", entityName);
                CreateFile("ZFR.Web\\Controllers\\" + entityName + "Controller.cs", "Controller.txt", entityName);

                //Dtos
                CreateFolder("ZFR.Web\\Dtos\\" + entityName);
                CreateFile("ZFR.Web\\Dtos\\" + entityName + "\\" + entityName + "ItemDto.cs", "ItemDto.txt", entityName);
                CreateFile("ZFR.Web\\Dtos\\" + entityName + "\\Create" + entityName + "Request.cs", "CreateRequest.txt", entityName);
                CreateFile("ZFR.Web\\Dtos\\" + entityName + "\\Update" + entityName + "Request.cs", "UpdateRequest.txt", entityName);

                //Frontend
                var entityNameLower = ToDashSeperated(entityName);
                CreateFile("ZFR.Web\\ClientApp\\src\\app\\models\\" + entityNameLower + ".model.ts", "WebModel.txt", entityName);
                CreateFile("ZFR.Web\\ClientApp\\src\\app\\services\\" + entityNameLower + ".service.ts", "WebService.txt", entityName);

                var pagePath = "ZFR.Web\\ClientApp\\src\\app\\modules\\genel\\" + entityNameLower;
                CreateFolder(pagePath);
                CreateFile(pagePath + "\\" + entityNameLower + ".component.html", "WebPageHtml.txt", entityName);
                CreateFile(pagePath + "\\" + entityNameLower + ".component.scss", "WebPageCss.txt", entityName);
                CreateFile(pagePath + "\\" + entityNameLower + ".component.ts", "WebPage.txt", entityName);

                Console.WriteLine("Completed successfully!");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("1. DbContexte eklemeyi unutmayin!");
                Console.WriteLine("2. DbContextde configuration eklemeyi unutmayin!");
                Console.WriteLine("3. Componenti genel module eklemeyi unutmayin!");
                Console.WriteLine("4. Genel routing altina yeni route eklenecek!");
                Console.WriteLine("5. Sol menu icin tanim yapmayi unutmayin!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void CreateFolder(string path)
        {
            var destination = mainPath + path;
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
                Console.WriteLine("Dizin " + destination + " oluşturuldu.");
            }
        }

        private static void CreateFile(string path, string fileName, string entityName)
        {
            var destination = mainPath + path;
            if (!File.Exists(destination))
            {
                File.Create(destination).Close();
            }

            var content = File.ReadAllText(".\\Samples\\" + fileName);
            content = content.Replace("#Entity#", entityName).Replace("#EntityLower#", ToDashSeperated(entityName)).Replace("#EntityCamelCase#", ToCamelCase(entityName))
                             .Replace("#UPDATE_FIELDS#", updateFields)
                             .Replace("#FIELDS#", entityFields)
                             .Replace("#DTO_FIELDS#", dtoFields)
                             .Replace("#WEB_FIELDS#", webFields);

            File.WriteAllText(destination, content, Encoding.UTF8);

            Console.WriteLine(destination + " oluşturuldu.");
        }

        private static string GetFields(string[] fields, string type)
        {
            var result = new StringBuilder();

            foreach (var field in fields)
            {
                var fieldType = field.Split(':')[0];
                var fieldName = field.Split(':')[1];

                if (type == "update")
                {
                    result.AppendLine(string.Format("existing.{0} = model.{0};", fieldName));
                }
                else if (type == "entity")
                {
                    result.AppendLine(string.Format("public {0} {1} ", fieldType, fieldName) + "{ get; set; }");
                }
                else if (type == "dto")
                {
                    result.AppendLine(string.Format("{0} = model.{0},", fieldName));
                }
                else if (type == "web-fields")
                {
                    result.AppendLine(string.Format("{0}: {1};", fieldName.ToLower(), fieldType.ToLower()));
                }
            }

            return result.ToString();
        }

        private static string ToDashSeperated(string value)
        {
            return Regex.Replace(value, @"([a-z])([A-Z])", "$1-$2").ToLower();
        }

        private static string ToCamelCase(string value)
        {
            return char.ToLowerInvariant(value[0]) + value.Substring(1);
        }
    }
}
