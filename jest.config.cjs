module.exports = {
  preset: "ts-jest", // Use ts-jest to transform TypeScript files
  testEnvironment: "node", // Set test environment to Node.js
  transform: {
    "^.+\\.tsx?$": "ts-jest", // Transform TypeScript files using ts-jest
  },
  moduleFileExtensions: ["ts", "tsx", "js", "jsx"], // Extensions to resolve
  transformIgnorePatterns: ["<rootDir>/node_modules/"], // Ignore transforming node_modules
  globals: {
    "ts-jest": {
      isolatedModules: true, // Use this for fast transformation, especially with large projects
    },
  },
};
