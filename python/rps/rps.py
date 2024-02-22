import random

def rps():
	player = input("Enter a choice (rock, paper, scissors) or (r, p, s) : ")
	computer = random.choice(["rock", "paper", "scissors"])
	full = "rock" if player=="r" else "paper" if player=="p" else "scissors"
	player = full if len(player)<2 else player
	print(f"\nYou chose {player}, the computer chose {computer}.")
	result = ""
	won = "\n:D You won the computer!"
	lost = "\n:/ You lost to the computer."
	paper = "Paper covers rock!"
	rock = "Rock smashes scissors!"
	scissors = "Scissors cut paper!"

	if player[0] == computer[0]:
		result = "It's a tie!"
	elif player == "rock":
		result = rock+won if computer == "scissors" else paper+lost
	elif player == "paper":
		result = scissors+lost if computer == "scissors" else paper+won
	elif player == "scissors":
		result = scissors+won if computer == "paper" else rock+lost
	else:
		result = "Sorry, what did you chose ?"
	
	print(result+"\n")
	print("Let's play again !")
	rps()

rps()
