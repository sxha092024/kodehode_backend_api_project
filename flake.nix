{
  description = "Kodehode backend - Group project c# rest api group - solo";

  inputs = {
    flake-utils.url = "github:numtide/flake-utils";
    nixpkgs.url = "github:nixos/nixpkgs/nixos-unstable"; # Rider is only 'free' in channels newer than 24.05
  };

  outputs = {
    self,
    nixpkgs,
    flake-utils,
  }:
    flake-utils.lib.eachDefaultSystem (
      system: let
        pkgs = import nixpkgs {
          inherit system;
          config.allowUnfree = true;
        };

        dotnetPkg = with pkgs.dotnetCorePackages;
        # NOTE: the first sdk in the list is the one whose cli utility is present in the environment
          combinePackages [
            sdk_9_0
            aspnetcore_9_0
          ];
      in {
        devShells.default = pkgs.mkShell {
          buildInputs = with pkgs; [];

          nativeBuildInputs = with pkgs; [
            dotnetPkg
            csharp-ls

            omnisharp-roslyn

            # Testing jetbrains rider IDE for C# now that it is 'free'
            # jetbrains.rider

            mermaid-cli

            sqlite
          ];
          shellHook = ''
            export DOTNET_ROOT="${dotnetPkg}";
            echo ".net root: '${dotnetPkg}'"
            echo ".net version: $(${dotnetPkg}/bin/dotnet --version)"
            echo ".net SDKs:"
            echo "$(${dotnetPkg}/bin/dotnet --list-sdks)"
            echo "sqlite: $(${pkgs.sqlite}/bin/sqlite3 --version)"
            echo "Flake shell active..."
          '';
        };
      }
    );
}
