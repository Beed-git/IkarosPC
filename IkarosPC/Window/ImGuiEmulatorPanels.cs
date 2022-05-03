using ImGuiNET;
using System;

namespace IkarosPC.Window;

internal class ImGuiEmulatorPanels
{
    private PC _pc;

    public ImGuiEmulatorPanels(PC pc)
    {
        _pc = pc;
    }

    public void MainWindow()
    {
        ImGui.DockSpaceOverViewport();
    }

    public void EmuWindow()
    {
        if (ImGui.Begin("Emulator"))
        {

        }
        ImGui.End();
    }

    public void RegistersWindow()
    {
        ImGui.ShowDemoWindow();

        if (ImGui.Begin("Registers"))
        {
            ImGui.Text($"PC: {_pc.Registers.PC.ToString("X4")}");
            ImGui.Text($"SP: {_pc.Registers.SP.ToString("X4")}");
            ImGui.Text($"FP: {_pc.Registers.FP.ToString("X4")}");

            ImGui.Separator();

            ImGui.Text($"A: {_pc.Registers.A.ToString("X4")}");
            ImGui.SameLine();
            ImGui.Text($"B: {_pc.Registers.B.ToString("X4")}");

            ImGui.Text($"C: {_pc.Registers.C.ToString("X4")}");
            ImGui.SameLine();
            ImGui.Text($"D: {_pc.Registers.D.ToString("X4")}");

            ImGui.Text($"E: {_pc.Registers.E.ToString("X4")}");
            ImGui.SameLine();
            ImGui.Text($"X: {_pc.Registers.X.ToString("X4")}");

            ImGui.Text($"Y: {_pc.Registers.Y.ToString("X4")}");
            ImGui.SameLine();
            ImGui.Text($"Z: {_pc.Registers.Z.ToString("X4")}");
            
            ImGui.Separator();

            ImGui.Text("Flags:");

            ImGui.Text($"Z: {(_pc.Registers.Zero ? "T" : "F")}");
            ImGui.SameLine();
            ImGui.Text($"C: {(_pc.Registers.Carry ? "T" : "F")}");

            ImGui.Text($"N: {(_pc.Registers.Negative ? "T" : "F")}");
            ImGui.SameLine();
            ImGui.Text($"S: {(_pc.Registers.Signed ? "T" : "F")}");

            ImGui.Separator();

            ImGui.Text($"ACC: {_pc.Registers.Accumulator.ToString("X4")}");
            ImGui.Text($"MBC: {_pc.Registers.MBC.ToString("X4")}");
                        
            ImGui.Text($"Stack Frame Size: {_pc.Registers.StackFrameSize.ToString("X4")}");


        }
        ImGui.End();
    }
}
