using System.Collections.Generic;

namespace IconChanger
{
    public class INIReader
    {
        public List<INIAsset> Assets = new List<INIAsset>();

        public void LoadFile(string File)
        {
            if (System.IO.File.Exists(File))
            {
                string[] Lines = System.IO.File.ReadAllLines(File);
                foreach (string Line in Lines)
                {
                    if (!Line.StartsWith("#") && !Line.StartsWith(";") && Line.Contains("="))
                    {
                        Assets.Add(INIKey.FromLine(Line));
                    }
                    else
                    {
                        Assets.Add(new INIEntity() { Content = Line });
                    }
                }
            }
        }

        public void RemoveKey(string Key)
        {
            foreach (var ent in Assets)
            {
                if (ent.Type == INIAssetType.Key && ((INIKey)ent).Key.ToLower() == Key.ToLower())
                {
                    Assets.Remove(ent);
                    return;
                }
            }
        }
        public void WriteLine(string Line)
        {
            Assets.Add(new INIEntity() { Content = Line });
        }
        public bool HasAutorunsHeader()
        {
            return (Assets.Count != 0 && Assets[0].Type == INIAssetType.Content && ((INIEntity)Assets[0]).Content.ToLower() == "[autorun]");
        }
        public void WriteAutorunsHeader()
        {
            if (!HasAutorunsHeader())
            {
                Assets.Insert(0, new INIEntity() { Content = "[AutoRun]" });
            }
        }
        public void SetKey(string Key, string Value)
        {
            foreach (var ent in Assets)
            {
                if (ent.Type == INIAssetType.Key && ((INIKey)ent).Key.ToLower() == Key.ToLower())
                {
                    ((INIKey)ent).Value = Value;
                    return;
                }
            }
            int i = Assets.Count;
            foreach (var ent in Assets)
            {
                if (ent.Type == INIAssetType.Key)
                {
                    i = Assets.IndexOf(ent);
                }
            }
            Assets.Insert(i, new INIKey() { Key = Key, Value = Value });
        }

        public void SaveFile(string File)
        {
            List<string> Lines = new List<string>();
            foreach (INIAsset Asset in Assets)
            {
                Lines.Add(Asset.GetContent);
            }
            if (System.IO.File.Exists(File))
            {
                System.IO.File.SetAttributes(File, System.IO.FileAttributes.Normal);
            }
            System.IO.File.WriteAllLines(File, Lines);
        }
    }

    public abstract class INIAsset
    {
        public abstract INIAssetType Type { get; }
        public abstract string GetContent { get; }
    }

    public class INIEntity : INIAsset
    {
        public string Content;
        public override INIAssetType Type => INIAssetType.Content;

        public override string GetContent => Content;
    }

    public class INIKey : INIAsset
    {
        public override INIAssetType Type => INIAssetType.Key;

        public override string GetContent => $"{Key}={Value}";

        public string Key;
        public string Value;
        public static INIKey FromLine(string Line)
        {
            INIKey ret = new INIKey();
            ret.Key = Line.Split('=')[0];
            ret.Value = Line.Remove(0, ret.Key.Length + 1);
            return ret;
        }
    }

    public enum INIAssetType
    {
        Content = 1,
        Key = 2
    }
}