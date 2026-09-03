// This program checks my Rule of Lords card files for simple mistakes.

import { readFile, readdir } from "node:fs/promises";
import { join } from "node:path";

// This describes the basic information every card should have.
interface CardData {
    id: string;
    name: string;
    starLevel: number;
}

// Read one JSON file and turn the information into card data.
async function loadCards(filePath: string): Promise<CardData[]> {
    const fileContent = await readFile(filePath, "utf-8");

    const parsedData = JSON.parse(fileContent);

    // I want every card file to contain a list, not just one single object.
    if (!Array.isArray(parsedData)) {
        throw new Error("JSON file must contain a list of cards");
    }

    return parsedData;
}

// I use this class to keep the different card checks together.
class CardValidator {
    // Check the basic fields of one card and collect any errors I find.
    validateCard(card: CardData): string[] {
        const errors: string[] = [];

        if (!card.id) {
            errors.push("Missing id");
        }

        if (!card.name) {
            errors.push("Missing name");
        }

        if (![0, 1, 2].includes(card.starLevel)) {
            errors.push("Invalid star level");
        }

        return errors;
    }

    // Remember IDs I already saw so I can find duplicate cards.
    findDuplicateIds(cards: CardData[]): string[] {
        const seenIds: string[] = [];
        const duplicateIds: string[] = [];

        for (const card of cards) {
            if (seenIds.includes(card.id)) {
                duplicateIds.push(card.id);
            } else {
                seenIds.push(card.id);
            }
        }

        return duplicateIds;
    }
}

// Search through a folder and its subfolders for JSON files.
async function findJsonFiles(folderPath: string): Promise<string[]> {
    const files: string[] = [];

    const entries = await readdir(folderPath, { withFileTypes: true });

    for (const entry of entries) {
        const fullPath = join(folderPath, entry.name);

        if (entry.isDirectory()) {
            // If I find another folder, I search inside it too. This is the recursive part.
            const nestedFiles = await findJsonFiles(fullPath);
            files.push(...nestedFiles);
        } else if (entry.isFile() && entry.name.endsWith(".json")) {
            files.push(fullPath);
        }
    }

    return files;
}

// This is the main part of the program where everything is put together.
async function main() {
    const jsonFiles = await findJsonFiles("data/cards");
    const validator = new CardValidator();
    const allCards: CardData[] = [];
    let validCards = 0;
    let cardsWithErrors = 0;

    // Load every JSON file I found. If one file is bad, the program can continue with the others.
    for (const file of jsonFiles) {
        try {
            const cards = await loadCards(file);
            allCards.push(...cards);
        } catch (error) {
            console.log(`Error loading ${file}`);

            if (error instanceof Error) {
                console.log(`- ${error.message}`);
            }
        }
    }

    console.log(`Cards loaded: ${allCards.length}`);

    // Check every loaded card and count how many have field errors.
    for (const card of allCards) {
        const errors = validator.validateCard(card);

        if (errors.length === 0) {
            validCards++;
        } else {
            cardsWithErrors++;
            console.log(`${card.id} - ${errors.join(", ")}`);
        }
    }

    // Duplicate IDs are checked separately from the normal field errors.
    const duplicateIds = validator.findDuplicateIds(allCards);

    // Show a short summary so it is easy to see if the card data has problems.
    console.log("\nValidation Summary");
    console.log(`Cards loaded: ${allCards.length}`);
    console.log(`Valid cards: ${validCards}`);
    console.log(`Cards with field errors: ${cardsWithErrors}`);
    console.log(`Duplicate IDs: ${duplicateIds.length}`);

    if (duplicateIds.length > 0) {
        console.log("\nDuplicate IDs:");

        for (const id of duplicateIds) {
            console.log(`- ${id}`);
        }
    }
}

main();
