void Main(string argument) {
   //  [Components]
   List<IMyTerminalBlock> componentStorage = new List<IMyTerminalBlock>();
   GridTerminalSystem.SearchBlocksOfName("[Components]", componentStorage);
   //  [Ingots]
   List<IMyTerminalBlock> ingotsStorage = new List<IMyTerminalBlock>();
   GridTerminalSystem.SearchBlocksOfName("[Ingots]", ingotsStorage);
   //  [Ores]
   List<IMyTerminalBlock> oresStorage = new List<IMyTerminalBlock>();
   GridTerminalSystem.SearchBlocksOfName("[Ores]", oresStorage);
   //  [Ref]
   List<IMyTerminalBlock> refinerys = new List<IMyTerminalBlock>();
   GridTerminalSystem.SearchBlocksOfName("[Ref]", refinerys);

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

            for (int o = inv.Count - 1; o >= 0; o--) {
               if (inv[o].Content.ToString().Contains("Ore")) {
                  var amount = (VRage.MyFixedPoint)((float)inv[o].Amount / refinerys.Count);

                  for (int b = 0; b < refinerys.Count; b++) {
                     if (b + 1 == refinerys.Count) {
                        amount = inv[o].Amount;
                     }

                     oreStorage.GetInventory(i).TransferItemTo(refinerys[b].GetInventory(0), o, null, true, amount);
                  }
               }
            }
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

         if (assembler.IsQueueEmpty || !assembler.IsProducing) {
            for (int o = 0; o < inv.Count; o++) {
               if (inv[o].Content.ToString().Contains("Ingot"))
                 moveToStorage(ingotsStorage, inv[o], assembler.GetInventory(i), o);
            }
         }

         for (int o = 0; o < inv.Count; o++) {
            if (!inv[o].Content.ToString().Contains("Ingot"))
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
