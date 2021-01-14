using UnityEngine;
using SFB;
using System.IO;

public class LevelMaker : MonoBehaviour{

    string text;

    [System.Serializable]
    class ColorToText{
        public Color color;
        public char character;
    }

    [SerializeField] char transparentCharacter = default;

    [SerializeField] ColorToText[] colorMappings = default;

    public void SelectFile(){
        var extensions = new[] {
            new ExtensionFilter("PNG", "png"),
            new ExtensionFilter("JPG", "jpg", "jpeg", "jpe", "jfif"),
            new ExtensionFilter("Bitmap", "bmp"),
            new ExtensionFilter("TIFF", "tiff", "tif"),
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", Application.dataPath, extensions, false);

        if(paths.Length != 0){
            var rawData = System.IO.File.ReadAllBytes(paths[0]);
            Texture2D map = new Texture2D(2, 2);
            map.LoadImage(rawData);

            string textImage = string.Empty;

            for (int y = map.height - 1; y >= 0; y--){
                for (int x = 0; x < map.width; x++){
                    Color pixelColor = map.GetPixel(x, y);

                    if(pixelColor.a == 0){
                        textImage += transparentCharacter;
                    }else{
                        textImage += PaintTile(pixelColor);
                    }
                }
                if(y != 0)
                    textImage += "\n";
            }

            text = textImage;
        }
    }

    char PaintTile(Color color){
        foreach(ColorToText col in colorMappings){
            if(col.color.Equals(color)){
                return col.character;
            }
        }
        return transparentCharacter;
    }

    public void SaveFile(){
        if (string.IsNullOrEmpty(text))
            return;

        var path = StandaloneFileBrowser.SaveFilePanel("Save Tiletext", Application.dataPath, "Untitled Tiletext", "txt");
        if (!string.IsNullOrEmpty(path)){
            StreamWriter stream = new StreamWriter(path, false);
            stream.Write(text);
            stream.Close();
        }
    }
}
