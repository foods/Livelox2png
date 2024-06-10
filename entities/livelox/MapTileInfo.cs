using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Livelox2png.entities.livelox;

internal class MapTileInfo
{
    public required List<MapTile> MapTiles { get; set; }
    public required ImageInfo ImageInfo { get; set; }
}
