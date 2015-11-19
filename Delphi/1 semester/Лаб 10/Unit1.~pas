unit Unit1;

interface

uses
  Windows, Messages, SysUtils, Variants, Classes, Graphics, Controls, Forms,
  Dialogs, ExtCtrls, TrainLib;

type
  TForm1 = class(TForm)
    Image1: TImage;
    Timer1: TTimer;
    procedure FormCreate(Sender: TObject);
    procedure FormDestroy(Sender: TObject);
    procedure Timer1Timer(Sender: TObject);
  private
    { Private declarations }
  public
    { Public declarations }
  end;

var
  Form1: TForm1;
  train : TTrain;

implementation

{$R *.dfm}

procedure TForm1.FormCreate(Sender: TObject);
begin
  train := TTrain.Create(Image1);
  Timer1.Enabled := true;
end;

procedure TForm1.FormDestroy(Sender: TObject);
begin
  train.Destroy;
end;

procedure TForm1.Timer1Timer(Sender: TObject);
begin
  train.draw;
  Image1.Update;
end;

end.
