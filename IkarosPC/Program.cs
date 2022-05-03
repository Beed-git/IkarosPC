using IkarosPC;
using IkarosPC.Window;

var pc = new PC();
var window = new EmuWindow(pc);
window.Start();