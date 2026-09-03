# Overview

I created a small Rule of Lords Content Validator in TypeScript. The program searches through folders for JSON card files, loads the cards, and checks them for simple problems. It can find missing names or IDs, invalid star levels, wrong ID formats, star levels that do not match the ID, and duplicate card IDs.

I wanted to build something small that could also be useful for my Rule of Lords project later. My main goal was to get more comfortable with TypeScript and learn how to work with classes, arrays, asynchronous functions, recursion, JSON files, and error handling in one program.

The validator also gives a short summary in the terminal so I can quickly see how many cards were loaded, how many cards are valid, and what problems were found.

[Software Demo Video](VIDEO_LINK_HERE)

# Development Environment

I used Visual Studio Code on Ubuntu to write and test the program. I also used the terminal, Git, and GitHub to manage and publish the project.

The program is written in TypeScript and runs with Node.js. I used npm to manage the project and `tsx` to run the TypeScript file. I also used Node's built-in `fs/promises` library to read files asynchronously and `path` to work with folder and file paths.

# Useful Websites

- [TypeScript Documentation](https://www.typescriptlang.org/docs/)
- [Node.js File System Documentation](https://nodejs.org/api/fs.html)
- [Node.js Path Documentation](https://nodejs.org/api/path.html)

# Future Work

- Add more card fields so the validator can check more of the real Rule of Lords card data.
- Show which file a card error came from to make problems easier to find.
- Add more automated tests for valid and invalid card data.
