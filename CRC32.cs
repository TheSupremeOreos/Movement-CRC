/*IDB Methods
////CField::LoadMap
//// -> CMapLoadable::LoadMap -> CWvsPhysicalSpace2D::Load -> m_dwCRC (Constant + Footholds + LadderRope?)
//// -> CPortalList::RestorePortal  -> m_dwPortalCrc (Portals)
//// -> CMapLoadable::LoadMap (Map Info)
*/


private static uint CalcCRC32String(string str)
{
    byte[] strArr = Encoding.ASCII.GetBytes(str);
    return CalcCRC32(strArr);
}

public static uint calcMapCRC(int mapID)
{
    uint crc = 0;
    MapleMap map;
    if (Database.mapLibrary.ContainsKey(mapID))
    {
        uint restorePortalCRC = 0;
        uint FHandLadderRopeCRC = 0;
        map = Database.mapLibrary[mapID];
        restorePortalCRC = CPortalListRestorePortalCRC(map);
        FHandLadderRopeCRC = CWvsPhysicalSpace2DLoadCRC(map);
        crc = FHandLadderRopeCRC ^ restorePortalCRC;
        crc = mapCRC(map, crc);
    }
    else
    {
        Database.loadMap(mapID);
        crc = calcMapCRC(mapID);
    }
    return crc;
}

private static uint mapCRC(MapleMap map, uint currentcrc)
{
    uint crc = currentcrc;
    crc = CalcCRC32(BitConverter.GetBytes(map.town)) ^ crc;
    crc = CalcCRC32(BitConverter.GetBytes(map.swim)) ^ crc;
    crc = CalcCRC32(BitConverter.GetBytes(map.fly)) ^ crc;
    crc = CalcCRC32(BitConverter.GetBytes(map.personalShop)) ^ crc;
    crc = CalcCRC32(BitConverter.GetBytes(map.ridingMove)) ^ crc;
    crc = CalcCRC32(BitConverter.GetBytes(0)) ^ crc; //fieldForce??? 
    crc = CalcCRC32(BitConverter.GetBytes(map.forceJump)) ^ crc;
    return crc;
}

private static uint CWvsPhysicalSpace2DGetConstantCRC()
{
    uint dwCRC = 0;
    int[] crcConstants = new int[] { Program.mapleVersion, 140000, 125, 80000, 60000, 120, 100000, 0, 120000, 140, 120000, 200, 2000, 670, 555, 2, 0, 0, 0 };
    foreach (int x in crcConstants)
    {
        dwCRC = CalcCRC32(BitConverter.GetBytes(x)) ^ dwCRC;
    }
    return dwCRC;
}
private static uint CPortalListRestorePortalCRC(MapleMap map, int mapID)
{
    uint dwPortalCrc = CalcCRC32(BitConverter.GetBytes(mapID));
    List<Portal> portals = map.getPortalList();
    foreach (Portal p in portals)
    {
        byte[] xyCoord = combineArrays(BitConverter.GetBytes((int)p.X), BitConverter.GetBytes(((int)p.Y)));
        byte[] onlyOnce = new byte[] { (byte)p.onlyOnce };

        dwPortalCrc = CalcCRC32String(p.pn) ^ dwPortalCrc;
        dwPortalCrc = CalcCRC32(BitConverter.GetBytes(p.pt)) ^ dwPortalCrc;
        dwPortalCrc = CalcCRC32(xyCoord) ^ dwPortalCrc;
        dwPortalCrc = CalcCRC32(BitConverter.GetBytes(p.hRange)) ^ dwPortalCrc;
        dwPortalCrc = CalcCRC32(BitConverter.GetBytes(p.VRange)) ^ dwPortalCrc;
        dwPortalCrc = CalcCRC32(BitConverter.GetBytes(p.tm)) ^ dwPortalCrc;
        dwPortalCrc = CalcCRC32String(p.tn) ^ dwPortalCrc;
        dwPortalCrc = CalcCRC32(BitConverter.GetBytes(p.delay)) ^ dwPortalCrc;
        dwPortalCrc = CalcCRC32(onlyOnce) ^ dwPortalCrc;
        dwPortalCrc = CalcCRC32(BitConverter.GetBytes(p.verticalImpact)) ^ dwPortalCrc;
        dwPortalCrc = CalcCRC32(BitConverter.GetBytes(p.horizontalImpact)) ^ dwPortalCrc;
    }
    return dwPortalCrc;
}

private static byte[] combineArrays(byte[] arr1, byte[] arr2)
{
    byte[] array1 = arr1;
    byte[] array2 = arr2;
    byte[] newArray = new byte[array1.Length + array2.Length];
    Array.Copy(array1, newArray, array1.Length);
    Array.Copy(array2, 0, newArray, array1.Length, array2.Length);
    return newArray;
}

private static uint CWvsPhysicalSpace2DLoadCRC(MapleMap map) //Footholds CRC?
{
    uint dwCrc = CWvsPhysicalSpace2DGetConstantCRC();
    List<Foothold> footholds = map.footholds.getFootHoldTree();
    foreach (Foothold f in footholds)
    {
        dwCrc = CalcCRC32(BitConverter.GetBytes(f.getX1())) ^ dwCrc;
        dwCrc = CalcCRC32(BitConverter.GetBytes(f.getY1())) ^ dwCrc;
        dwCrc = CalcCRC32(BitConverter.GetBytes(f.getX2())) ^ dwCrc;
        dwCrc = CalcCRC32(BitConverter.GetBytes(f.getY2())) ^ dwCrc;
        dwCrc = CalcCRC32(BitConverter.GetBytes(f.getDrag())) ^ dwCrc;
        dwCrc = CalcCRC32(BitConverter.GetBytes(f.getForce())) ^ dwCrc;
        dwCrc = CalcCRC32(BitConverter.GetBytes(f.getForbidFallDown())) ^ dwCrc;
        dwCrc = CalcCRC32(BitConverter.GetBytes(f.getCantThrough())) ^ dwCrc; //piece?
        dwCrc = CalcCRC32(BitConverter.GetBytes(f.getPrev())) ^ dwCrc;
        dwCrc = CalcCRC32(BitConverter.GetBytes(f.getNext())) ^ dwCrc;
        dwCrc = CalcCRC32(BitConverter.GetBytes(f.getId())) ^ dwCrc;
    }
    //dwCrc = 0;
    if (map.ladderRope.Count > 0)
    {
        List<LadderRope> ladderRope = map.ladderRope;
        foreach (LadderRope props in ladderRope)
        {
            dwCrc = CalcCRC32(BitConverter.GetBytes(props.ID)) ^ dwCrc;
            dwCrc = CalcCRC32(BitConverter.GetBytes(props.l)) ^ dwCrc; //pData?
            dwCrc = CalcCRC32(BitConverter.GetBytes(props.uf)) ^ dwCrc; //xMax?
            dwCrc = CalcCRC32(BitConverter.GetBytes(props.y1)) ^ dwCrc; //prev?
            dwCrc = CalcCRC32(BitConverter.GetBytes(props.y2)) ^ dwCrc; //next?
            dwCrc = CalcCRC32(BitConverter.GetBytes(props.x)) ^ dwCrc; //next?
            dwCrc = CalcCRC32(BitConverter.GetBytes(props.page)) ^ dwCrc; //next?
        }
    }
    return dwCrc;
}