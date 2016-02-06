void Main(string argument) { 
   //  [Components] 
   List<IMyTerminalBlock> componentStorage = new List<IMyTerminalBlock>(); 
   GridTerminalSystem.SearchBlocksOfName("[Main]", componentStorage); 
   //  [Ingots]                                         
   List<IMyTerminalBlock> ingotsStorage = new List<IMyTerminalBlock>(); 
   GridTerminalSystem.SearchBlocksOfName("[Main]", ingotsStorage); 
   //  [Ores]                     
   List<IMyTerminalBlock> oresStorage = new List<IMyTerminalBlock>(); 
   GridTerminalSystem.SearchBlocksOfName("[Main]", oresStorage); 
                        
   List<IMyTerminalBlock> arcFurnaces = new List<IMyTerminalBlock>(); 
   List<IMyTerminalBlock> refinerys = new List<IMyTerminalBlock>(); 

   List<IMyTerminalBlock> refs = new List<IMyTerminalBlock>(); 
   GridTerminalSystem.GetBlocksOfType <IMyRefinery>(refs); 

   for (int r = 0; r < refs.Count; r++) { 
      if (refs[r].DetailedInfo.StartsWith("Type: Ref")) {
         refinerys.Add(refs[r]);
      } else if (refs[r].DetailedInfo.StartsWith("Type: Arc")) {
         arcFurnaces.Add(refs[r]);
      }  
   } 

   List<IMyTerminalBlock> assemblers = new List<IMyTerminalBlock>(); 
   GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers); 

   List<IMyTerminalBlock> cargoContainters = new List<IMyTerminalBlock>(); 
   GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoContainters); 

   //Move all ores to the Ore Storage 
   for (int c = 0; c < cargoContainters.Count; c++) { 
      IMyCargoContainer cargo = (IMyCargoContainer)cargoContainters[c]; 
      if (!oresStorage.Contains(cargo)) { 
         for (int i = 0; i < cargo.GetInventoryCount(); i++) { 
            List<IMyInventoryItem> inv = cargo.GetInventory(i).GetItems(); 
            for (int o = 0; o < inv.Count; o++) { 
               if (inv[o].Content.ToString().Contains("Ore")) { 
                  moveToStorage(oresStorage, inv[o], cargo.GetInventory(i), o); 
               } 
            } 
         } 
      } 
   }                         
   //Share ores between all of the refinerys and arc furnaces 
   for (int os = 0; os < oresStorage.Count; os++) { 
      if (oresStorage[os] is IMyCargoContainer) { 
         IMyCargoContainer oreStorage = (IMyCargoContainer)oresStorage[os]; 
         for (int i = 0; i < oreStorage.GetInventoryCount(); i++) { 
            List<IMyInventoryItem> inv = oreStorage.GetInventory(i).GetItems(); 
            for (int o = 0; o < inv.Count; o++) { 
               if (inv[o].Content.SubtypeName.Contains("Iron") 
               || inv[o].Content.SubtypeName.Contains("Nickel") 
               || inv[o].Content.SubtypeName.Contains("Cobalt")) { 
                  var amount = (VRage.MyFixedPoint)((float)inv[o].Amount / arcFurnaces.Count); 
                  for (int b = 0; b < arcFurnaces.Count; b++) 
                      oreStorage.GetInventory(i).TransferItemTo(arcFurnaces[b].GetInventory(0), o, null, true, amount); 
               } else { 
                  var amount = (VRage.MyFixedPoint)((float)inv[o].Amount / refinerys.Count); 
                  for (int b = 0; b < refinerys.Count; b++) 
                      oreStorage.GetInventory(i).TransferItemTo(refinerys[b].GetInventory(0), o, null, true, amount); 
               } 
            } 
         } 
      } 
   } 
   //Move All Ingots out of the arc furnaces 
   for (int bf = 0; bf < arcFurnaces.Count; bf++) { 
      for (int i = 0; i < arcFurnaces[bf].GetInventoryCount(); i++) { 
         List<IMyInventoryItem> inv = arcFurnaces[bf].GetInventory(i).GetItems(); 
         for (int o = 0; o < inv.Count; o++) { 
            if (!inv[o].Content.ToString().Contains("Ore")) 
              moveToStorage(ingotsStorage, inv[o], arcFurnaces[bf].GetInventory(i), o); 
         } 
      } 
   } 
   //Move All Ingots out of the refinerys 
   for (int f = 0; f < refinerys.Count; f++) { 
      for (int i = 0; i < refinerys[f].GetInventoryCount(); i++) { 
         List<IMyInventoryItem> inv = refinerys[f].GetInventory(i).GetItems(); 
         for (int o = 0; o < inv.Count; o++) { 
            if (!inv[o].Content.ToString().Contains("Ore")) 
              moveToStorage(ingotsStorage, inv[o], refinerys[f].GetInventory(i), o); 
         } 
      } 
   } 
   //Empty out assemblers 
   for(int a = 0; a < assemblers.Count; a ++) { 
      IMyAssembler assembler = (IMyAssembler) assemblers[a]; 
      for(int i = 0; i < assembler.GetInventoryCount(); i ++) { 
         List<IMyInventoryItem> inv = assembler.GetInventory(i).GetItems(); 
         for (int o = 0; o < inv.Count; o++) { 
            if (inv[o].Content.ToString().Contains("Ingot") && (assembler.IsQueueEmpty || !assembler.IsProducing)) 
              moveToStorage(ingotsStorage, inv[o], assembler.GetInventory(i), o); 
            else if (!inv[o].Content.ToString().Contains("Ingot")) 
              moveToStorage(componentStorage, inv[o], assembler.GetInventory(i), o); 
         }     
      } 
   } 
} 

public void moveToStorage(List<IMyTerminalBlock> storage, IMyInventoryItem item,IMyInventory inv,int index) { 
   var amount = (VRage.MyFixedPoint)((float)item.Amount / storage.Count); 

   for (int i = 0; i < storage.Count; i++) { 
      if (storage[i] is IMyCargoContainer) { 
         storage[i].GetInventory(0).TransferItemFrom(inv, index, null, true, amount); 
      } 
   } 
}
