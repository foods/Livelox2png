using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livelox2png.entities.livelox;

internal class TileData
{
    public required MapTileInfo MapTileInfo { get; set; }
    public required ImageInfo ImageInfo { get; set; }
    public string ImageFormat { get; set; } = "";
    public string MediaType { get; set; } = "";
    public bool Success { get; set; }

}
