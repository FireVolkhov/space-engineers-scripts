//Для функционирования кода понадобится текстовая панель или ЖК-панель с названием Монитор (по умолчанию), 
//а также таймер запускающий этот код раз в секунду. 
 
double codeExecutionRate = 1; //1 потому-что заточено под выполнение раз в секунду 
double KoofSkorosti = 1.3333; //Коофициент скорости нужен потому-что сетка GPS в SE походу не в метрах. 
private static double PPosX = 0; //Пременные в которых хранятся 
private static double PPosY = 0; //результаты предыдущего 
private static double PPosZ = 0; //замера координат 
private static double PSpeed = 0; //и скорости 
 
public Double changeIn(Double previous, Double current){ 
  return (current-previous)/codeExecutionRate; 
} 
 
public Double magnitude(Double x, Double y, Double z){ 
  return Math.Sqrt((Math.Pow(x,2)+Math.Pow(y,2)+Math.Pow(z,2))); 
} 
 
void Main(){ 
  IMyTextPanel Infa = (IMyTextPanel)GridTerminalSystem.GetBlockWithName("LCD"); 
  //При необходимости замените слово Монитор названием своей текстовой или LCD панели 
   
  //Получаем текущие координаты: 
  double X = (Infa.GetPosition().GetDim(0)); 
  double Y = (Infa.GetPosition().GetDim(1)); 
  double Z = (Infa.GetPosition().GetDim(2)); 
        
  String Koordinati = "Координаты: \n X: " + Convert.ToString(Math.Round(X,1)) + 
  "\n Y: " + Convert.ToString(Math.Round(Y,1)) + 
  "\n Z: " + Convert.ToString(Math.Round(Z,1)); //Заодно округляем, и улучшаем восприятие инфы разделяя на строчки 
        
  //Получаем скорость: 
  double velocityX = changeIn(PPosX,X); 
  double velocityY = changeIn(PPosY,Y); 
  double velocityZ = changeIn(PPosZ,Z); 
  double Speed = magnitude(velocityX,velocityY,velocityZ)/KoofSkorosti; 
   
  String Skorost = "Скорость: "+ Convert.ToString(Math.Round(Speed,2))+" м/с"; 
   
  //Получаем ускорение: 
  String Uskorenie = "Ускорение: "+ Convert.ToString(Math.Round(Speed-PSpeed,2))+" м/с"; 
   
  //Обновляем предыдущие координаты и скорость: 
  PPosX = X; 
  PPosY = Y; 
  PPosZ = Z; 
  PSpeed = Speed; 
   
  //Выводим инфу на монитор 
  Infa.WritePublicText(Skorost + "\n\n" + Uskorenie + "\n\n" + Koordinati, false); 
  Infa.ShowTextureOnScreen(); 
  Infa.ShowPublicTextOnScreen(); 
  
  IMyProgrammableBlock comp = (IMyProgrammableBlock)GridTerminalSystem.GetBlockWithName("Com2"); 
  comp.TryRun("Hello");
}
