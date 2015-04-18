#include "etherShield.h"
#include "ETHER_28J60.h"
static uint8_t mac[6] = {0x54, 0x55, 0x58, 0x10, 0x00, 0x24};
static uint8_t ip[4] = {192, 168,0, 115};
static uint16_t port = 80;
ETHER_28J60 e;
int temperatureValues[100];
int countTemperature=0;
int pompaPin=9;
int humidityPin=A0;
int temperaturePin=A1;
int luminaPin=A2;
int pragHumidity=750 ;
unsigned long start;
unsigned long startPompa;
unsigned long now;
bool isPompaOn=false;
unsigned long interval=60000,intervalPompa=10000;
void turnOnPompa()
{
  digitalWrite(pompaPin,HIGH);
/*  delay(4000);
  digitalWrite(pompaPin,LOW);*/
 now=millis();
  isPompaOn=true;
  startPompa=now;
}
void turnOffPompa()
{
  digitalWrite(pompaPin,LOW);
  isPompaOn=false;
}
int readHumidity()
{
  int value=0;
  int loops=10;
  for(int i=0;i<loops;++i)
  {
    value+=analogRead(humidityPin);
  }
  return value/loops;
}
int readLumina()
{
  int value=0;
  int loops=10;
  for(int i=0;i<loops;++i)
  {
    value+=analogRead(luminaPin);
  }
  return value/loops;
}
int readTemperature()
{
  int value=0;
  int loops=10;
  for(int i=0;i<loops;++i)
  {
    value+=analogRead(temperaturePin);
  }
  float voltage = value/loops * 5.0;
  voltage /= 1024.0;
  float temperatureCelsius = (voltage - 0.5) * 100 ;
  temperatureValues[countTemperature]=temperatureCelsius;
  countTemperature++;
  int sum=0;
  for(int i=0;i<countTemperature;++i)
  {
    sum+=temperatureValues[i];
  }
  return sum/countTemperature;
}
void CheckHumidityOk()
{
  if(readHumidity()<pragHumidity)
  {
    turnOnPompa();
  }
}
void setup() 
{
  e.setup(mac, ip, port);
  pinMode(pompaPin,OUTPUT);
  Serial.begin(9600);
  now=millis();
  start=now;
  //delay(1000);
}

void loop() 
{
  char* params;
  if (params = e.serviceRequest())
  {
    Serial.println("a");
    
    if (strcmp(params, "?cmd=temperature") == 0)
    {
      e.print(readTemperature());
      //Serial.println("b");
      //turnOnPompa();
      //e.setup(mac, ip, port);
    }
    else if (strcmp(params, "?cmd=humidity") == 0)
    {
      e.print(readHumidity());
    }
    else if (strcmp(params, "?cmd=lumina") == 0)
    {
      e.print(readLumina());
    }
    else if (strcmp(params, "?cmd=prag-udare") == 0)
    {
      e.print(pragHumidity);
    }
    else if (strcmp(params, "?cmd=uda") == 0)
    {
      e.print("udare in progres<br>");
      e.print("<a href='?cmd=no-command'>home</a> <a href='?cmd=stop-uda'>oprire udare</a><br>");
      turnOnPompa();
    }
    else if (strcmp(params, "?cmd=stop-uda") == 0)
    {
      e.print("udare in progres<br>");
      e.print("<a href='?cmd=no-command'>home</a> <a href='?cmd=uda'>uda</a><br>");
      startPompa=millis();
      turnOffPompa();
    }
    else if (strcmp(params, "?cmd=up-prag") == 0)
    {
      pragHumidity+=50;
    }
    else if (strcmp(params, "?cmd=down-prag") == 0)
    {
      pragHumidity-=50;
    }
    else
    {
      e.print("<H1>Web Remote</H1><a href='?cmd=uda'>uda</a> <a href=''>home</a> <a href='?cmd=up-prag'>Ridicare prag</a> <a href='?cmd=down-prag'>Scadere prag</a>");
      e.print("<br>umiditate:");
      e.print(readHumidity());
      e.print("<br>temperatura:");
      e.print(readTemperature());
      e.print("<br>lumina::");
      e.print(readLumina());
      e.print("<br>Parg de udare:");
      e.print(pragHumidity);
    }
    e.respond();
  }
  now=millis();
  int dif=now-start;
  if(dif>interval)
  {
    start=now;
    e.setup(mac, ip, port);
  }
  if(isPompaOn)
  {
    dif=now-startPompa;
    if(dif>intervalPompa)
    {
      startPompa=now;
      turnOffPompa();
    }
  }
  CheckHumidityOk();
  //Serial.println(readHumidity());
}
