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
            crc = CalcCRC32(BitConverter.GetBytes(map.forcedReturn)) ^ crc; //ForceField variable?
            crc = CalcCRC32(BitConverter.GetBytes(map.forceJump)) ^ crc; //ForceField.njump
            return crc;
        }

        private static uint CWvsPhysicalSpace2DGetConstantCRC()
        {
            uint dwCRC = 0;
            int[] crcConstants = new int[] { 140000, 125, 80000, 60000, 120, 100000, 0, 120000, 140, 120000, 200, 2000, 670, 555, 2, 0, 0, 0 };
            dwCRC = CalcCRC32(BitConverter.GetBytes(Program.mapleVersion));
            foreach (int x in crcConstants)
            {
                dwCRC = CalcCRC32(BitConverter.GetBytes(x)) ^ dwCRC;
            }
            return dwCRC;
        }
        private static uint CPortalListRestorePortalCRC(MapleMap map)
        {
            uint dwPortalCrc = CWvsPhysicalSpace2DGetConstantCRC();
            List<Portal> portals = map.getPortalList();
            foreach (Portal p in portals)
            {
                byte[] xyCoord = new byte[] { };
                xyCoord.Concat(BitConverter.GetBytes((int)p.X));
                xyCoord.Concat(BitConverter.GetBytes((int)p.Y));
                dwPortalCrc = CalcCRC32String(p.pn) ^ dwPortalCrc;
                dwPortalCrc = CalcCRC32(BitConverter.GetBytes(p.pt)) ^ dwPortalCrc;
                dwPortalCrc = CalcCRC32(xyCoord) ^ dwPortalCrc;
                dwPortalCrc = CalcCRC32(BitConverter.GetBytes(p.hRange)) ^ dwPortalCrc;
                dwPortalCrc = CalcCRC32(BitConverter.GetBytes(p.VRange)) ^ dwPortalCrc;
                dwPortalCrc = CalcCRC32(BitConverter.GetBytes(p.tm)) ^ dwPortalCrc;
                dwPortalCrc = CalcCRC32String(p.tn) ^ dwPortalCrc;
                dwPortalCrc = CalcCRC32(BitConverter.GetBytes(p.delay)) ^ dwPortalCrc;
                dwPortalCrc = CalcCRC32(BitConverter.GetBytes(p.onlyOnce)) ^ dwPortalCrc;
                dwPortalCrc = CalcCRC32(BitConverter.GetBytes(p.verticalImpact)) ^ dwPortalCrc;
                dwPortalCrc = CalcCRC32(BitConverter.GetBytes(p.horizontalImpact)) ^ dwPortalCrc;
            }
            return dwPortalCrc;
        }

        private static uint CWvsPhysicalSpace2DLoadCRC(MapleMap map) //Footholds CRC?
        {
            uint dwCrc = 0;
            List<Foothold> footholds = map.footholds.getFootHoldTree();
            foreach(Foothold f in footholds)
            {
                dwCrc = CalcCRC32(BitConverter.GetBytes(f.getX1())) ^ dwCrc;
                dwCrc = CalcCRC32(BitConverter.GetBytes(f.getY1())) ^ dwCrc;
                dwCrc = CalcCRC32(BitConverter.GetBytes(f.getX2())) ^ dwCrc;
                dwCrc = CalcCRC32(BitConverter.GetBytes(f.getY2())) ^ dwCrc;
                dwCrc = CalcCRC32(BitConverter.GetBytes(f.getDrag())) ^ dwCrc;
                dwCrc = CalcCRC32(BitConverter.GetBytes(f.getForce())) ^ dwCrc;
                dwCrc = CalcCRC32(BitConverter.GetBytes(f.getForbidFallDown())) ^ dwCrc;
                dwCrc = CalcCRC32(BitConverter.GetBytes(f.getCantThrough())) ^ dwCrc;
                dwCrc = CalcCRC32(BitConverter.GetBytes(f.getPrev())) ^ dwCrc;
                dwCrc = CalcCRC32(BitConverter.GetBytes(f.getNext())) ^ dwCrc;
                dwCrc = CalcCRC32(BitConverter.GetBytes(f.getId())) ^ dwCrc;
            }
            if (map.ladderRope.Count > 0) //THIS PART IS for sure
            {
                List<LadderRope> ladderRope = map.ladderRope;
                foreach (LadderRope props in ladderRope)
                {
                    dwCrc = CalcCRC32(BitConverter.GetBytes(props.l)) ^ dwCrc;
                    dwCrc = CalcCRC32(BitConverter.GetBytes(props.uf)) ^ dwCrc;
                    dwCrc = CalcCRC32(BitConverter.GetBytes(props.x)) ^ dwCrc;
                    dwCrc = CalcCRC32(BitConverter.GetBytes(props.y1)) ^ dwCrc;
                    dwCrc = CalcCRC32(BitConverter.GetBytes(props.y2)) ^ dwCrc;
                    dwCrc = CalcCRC32(BitConverter.GetBytes(props.page)) ^ dwCrc;
                    dwCrc = CalcCRC32(BitConverter.GetBytes(props.piece)) ^ dwCrc;
                }
            }
            return dwCrc;
        }
        