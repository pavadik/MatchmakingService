# Matchmaking Service 

This project is an implementation of a matchmaking service to distribute players into teams, taking into account their skill, network lag, and queue waiting time. The goal of the service is to form teams of a given size, minimizing the differences in skills and lag between players, and reducing the waiting time in the queue.

## Features
* **Multi-criteria team selection:** considering skill, network lag, and waiting time.
* **Flexible algorithm tuning** with the ability to adjust weighting coefficients for each parameter.
* **Priority service** for players with longer waiting times.
* **Computational complexity limitation** for efficient operation with a large number of players.

## Getting Started
Follow these instructions to get a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites
* **.NET SDK 6.0** or newer.
* An IDE like **Visual Studio 2022** or **Visual Studio Code** with the C# extension.

### Installation
1. **Clone the repository** to your local machine:
```
git clone https://github.com/pavadik/MatchmakingService.git
```
2. **Navigate to the project directory:`
```
cd MatchmakingService
```
3. **Restore dependencies** (if using NuGet packages):
```
dotnet restore
```

## Usage

The project contains an example of using the matchmaking service in the Main method of the Program class. You can run the project and observe the algorithm in action.

### Running the Project

Run the project using the command:
```
dotnet run
```

### Sample Output
```
Team:
Player 2, Skill: 1180, Lag: 50, Waiting: 2.005s
Player 3, Skill: 1220, Lag: 50, Waiting: 2.005s
Skill Difference: 40
Lag Difference: 0
Maximum Waiting Time: 2.005s

Team:
Player 1, Skill: 1050, Lag: 50, Waiting: 2.005s
Player 4, Skill: 1350, Lag: 50, Waiting: 2.005s
Skill Difference: 300
Lag Difference: 0
Maximum Waiting Time: 2.005s
```

### Adjusting Weighting Coefficients

You can adjust the influence of each parameter on team formation by changing the weighting coefficients in the CalculateTeamScore method of the MatchmakingService class:
```
const double skillWeight = 0.5;
const double lagWeight = 0.3;
const double waitingTimeWeight = 0.2;
```

## Project Structure
* **Player.cs:** Class representing a player with skill, lag, and waiting time parameters.
* **Team.cs:** Team class containing a list of players and methods for calculating parameter differences.
* **MatchmakingService.cs:** The main service containing the logic for forming teams.
* **Program.cs:** The entry point of the application with an example of using the service.

## Algorithm Workflow
1. **Player Collection:** Players are added to the service's waiting queue.
2. **Queue Sorting:** Players are sorted by waiting time, skill, and lag.
3. **Team Formation:**
* Possible combinations of players are generated for teams of the specified size.
* A "score" is calculated for each combination based on the weighting coefficients.
* The combination with the lowest score is selected.
4. **Constraints Handling:** To prevent high computational complexity, the number of combinations checked is limited.

## Contributing

If you want to improve the project, you can:

1. **Fork the repository.**

2. **Create a new feature branch:**
```
git checkout -b feature/NewFeature
```

3. **Make your changes and commit them:**
```
git commit -am 'Add new feature'
```

4. **Push your changes to your fork:**
```
git push origin feature/NewFeature
```

5. **Create a Pull Request for review and merging.**

## License
This project is licensed under the MIT License. See the LICENSE file for details.

## Contact
If you have any questions or suggestions, please contact the project author.

## Acknowledgments
Thanks to all members of the C# developer community for their contributions and support.
Special thanks for providing examples and problem statements that served as the basis for this project.
