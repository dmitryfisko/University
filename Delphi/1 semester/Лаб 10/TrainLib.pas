unit TrainLib;

interface

uses
  Windows, Graphics, DateUtils, SysUtils, Dialogs, ExtCtrls;

const
  MAX_CLOUDS_COUNT = 10000;

type

  TCloud = class
    public
      creationTime : TDateTime;
      stX, stY : integer;
      isInitialized : boolean;
      procedure init(x, y : integer; time : TDateTime);
  end;

  TClouds = class
    Constructor Create(imageCanvas : TCanvas; speed : extended);
    public
      procedure draw(timeNow : TDateTime; x, y : integer);
    private
      canvas : TCanvas;
      clouds : array[1..MAX_CLOUDS_COUNT] of TCloud;
      cloudPrevTime : TDateTime;
      cloudCounts : integer;
      trainSpeed : extended;
      function sqrtFun(x : extended) : integer;
  end;

  TBeam = class
    Constructor Create(imageCanvas : TCanvas);
    public
      procedure draw(timeNow : TDateTime; x, y : integer);
    private
      canvas : TCanvas;
  end;

  TTrain = class
    Constructor Create(image : TImage);
    public
      procedure draw;
    private
      clouds : TClouds;
      beam : TBeam;
      canvas : TCanvas;
      posX : extended;
      posY, height, width : integer;
      prevTime : TDateTime;
      procedure clearCanvas;
      procedure drawWay;
  end;

implementation

const
  TRAIN_START_X = -300;
  TRAIN_SPEED = 150.0;   //speed - px per second

  BEAM_ANIMATION_DURATION = 1500;

procedure TCloud.init;
begin
  stX := x;
  stY := y;
  creationTime := time;
  isInitialized := true;
end;

constructor TClouds.Create;
var
  i : integer;
begin
  canvas := imageCanvas;
  cloudPrevTime := -1;
  cloudCounts := 0;

  for i:=1 to MAX_CLOUDS_COUNT do begin
    clouds[i] := TCloud.Create;
    clouds[i].isInitialized := false;
  end;
end;

function TClouds.sqrtFun;
begin
  Result := round(11*sqrt(x));
end;

procedure TClouds.draw;
const
  CLOUD_ANIMATION_DURATION = 3000;
  CLOUD_PATH_WIDTH = 580;
  CLOUD_TIME_DISTANCE = 150;
  CLOUD_MAX_SCALE = 5;
var
  i, dif, cloudStX, offsetX, offsetY, add, rad, timeDif : integer;
  creationTime : TDateTime;
  color : integer;
begin
  if(cloudPrevTime = -1) then begin
    Inc(cloudCounts);
    clouds[cloudCounts].init(x, y, timeNow);
    cloudPrevTime := timeNow;
    add := 0;
  end else begin
    timeDif := MilliSecondsBetween(timeNow, cloudPrevTime);
    add := timeDif div CLOUD_TIME_DISTANCE;
  end;

  for i:=1 to add do begin
    //ShowMessage('created');
    creationTime := IncMilliSecond(cloudPrevTime, CLOUD_TIME_DISTANCE);
    dif := MilliSecondsBetween(timeNow, creationTime);
    cloudStX := round(x - trainSpeed*dif/1000);
    Inc(cloudCounts);
    add := add;
    clouds[cloudCounts].init(cloudStX, y, creationTime);
    cloudPrevTime := creationTime;
  end;

  with canvas do begin
    for i:=1 to MAX_CLOUDS_COUNT do begin
      if(not clouds[i].isInitialized) then continue;

      color := round(255 * (timeDif/CLOUD_ANIMATION_DURATION));
      if(color > 255) then color := 255;
      Pen.Color := RGB(color, color, color);
      Brush.Color := RGB(color, color, color);

      timeDif := MilliSecondsBetween(timeNow, clouds[i].creationTime);
      if(timeDif > CLOUD_ANIMATION_DURATION) then continue;
      offsetX := round(timeDif/CLOUD_ANIMATION_DURATION*CLOUD_PATH_WIDTH);
      offsetY := sqrtFun(offsetX);

      rad := 10;
      rad := round(rad*(1 + timeDif/CLOUD_ANIMATION_DURATION*CLOUD_MAX_SCALE));
      Ellipse(x-rad-offsetX, y-rad-offsetY, x+rad-offsetX, y+rad-offsetY);
    end;
  end;
end;

constructor TBeam.Create;
begin
  canvas := imageCanvas;
end;

procedure TBeam.draw;
const
  WHEEL_WIDTH = 140;
  WHEEL_HEIGHT = 8;
var
  rad, hour, min, sec, mSec : Word;
  xPoint, yPoint, millisec, halfHeight : integer;
begin
  DecodeTime(timeNow, hour, min, sec, mSec);
  millisec := hour*(1000*60*60) + min*(1000*60) +
      sec*(1000) + mSec;
  millisec := millisec mod BEAM_ANIMATION_DURATION;

  rad := 25;
  xPoint := round(x + rad*cos(2*Pi * millisec / BEAM_ANIMATION_DURATION));
  yPoint := round(y + rad*sin(2*Pi * millisec / BEAM_ANIMATION_DURATION));

  halfHeight := WHEEL_HEIGHT div 2;
  with canvas do begin
    Pen.Color := clRed;
    Brush.Color := clRed;

    rad := 8;
    Ellipse(xPoint-rad, yPoint-rad, xPoint+rad, yPoint+rad);
    Ellipse(xPoint-rad+WHEEL_WIDTH, yPoint-rad,
            xPoint+rad+WHEEL_WIDTH, yPoint+rad);

    Pen.Color := $C9C9C9;
    Brush.Color := $C9C9C9;
    Rectangle(xPoint, yPoint-halfHeight, xPoint+WHEEL_WIDTH, yPoint+halfHeight);

    rad := halfHeight;
    Ellipse(xPoint-rad, yPoint-rad, xPoint+rad, yPoint+rad);
    Ellipse(xPoint-rad+WHEEL_WIDTH, yPoint-rad,
            xPoint+rad+WHEEL_WIDTH, yPoint+rad);

  end;
end;

constructor TTrain.Create;
begin
  prevTime := Now;
  canvas := image.Canvas;
  height := image.Height;
  width := image.Width;

  clouds := TClouds.Create(canvas, TRAIN_SPEED);
  beam := TBeam.Create(canvas);

  posX := TRAIN_START_X;
  posY := round(height/3);
end;

procedure TTrain.draw;
var
  timeNow : TDateTime;
  x, y, rad : integer;
begin
  clearCanvas;
  drawWay;

  timeNow := Now;
  posX := posX + TRAIN_SPEED * MilliSecondsBetween(timeNow, prevTime) / 1000;
  if(posX > width + 280) then posX := TRAIN_START_X;
  x := round(posX); y := posY;
  clouds.draw(timeNow, x+220, y+50);

  with canvas do begin
    Brush.Color := $D97300;   //blue
    Pen.Color := $D97300;
    Rectangle(x, y, x+110, y+20);
    Rectangle(x+10, y+19, x+100, y+80);
    Rectangle(x, y+79, x+250, y+160);
    Rectangle(x+205, y+45, x+230, y+80);
    Rectangle(x+190, y+30, x+245, y+46);

    Brush.Color := $3540FF; //red
    Pen.Color := $3540FF;
    rad := 40;
    Ellipse(x-rad+55, y-rad+150, x+rad+55, y+rad+150);
    Ellipse(x-rad+195, y-rad+150, x+rad+195, y+rad+150);
    Brush.Color := $D97300;   //blue
    Pen.Color := $D97300;
    rad := 10;
    Ellipse(x-rad+55, y-rad+150, x+rad+55, y+rad+150);
    Ellipse(x-rad+195, y-rad+150, x+rad+195, y+rad+150);
  end;

  beam.draw(timeNow, x+55, y+150);

  prevTime := timeNow;
end;

procedure TTrain.drawWay;
begin
  with canvas do begin
    Brush.Color := $A9A9A9;
    Pen.Color := $A9A9A9;
    Rectangle(0, posY+186, width, posY+190);
    Brush.Color := $B5B5B5;
    Pen.Color := $B5B5B5;
    Rectangle(0, posY+190, width, posY+195);
  end;
end;

procedure TTrain.clearCanvas;
var
  skyHeight : integer;
begin
  with canvas do begin
    skyHeight := round(height/1.8);
    Brush.Color := $FFDB7E;
    Pen.Color := $FFDB7E;
    Rectangle(0, 0, width, skyHeight);
    Brush.Color := $6FFF01;
    Pen.Color := $6FFF01;
    Rectangle(0, skyHeight, width, height);
    //FillRect(ClipRect);
  end;
end;

end.
