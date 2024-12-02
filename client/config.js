const fs = require('fs');
const path = require('path');

//VERCEL DEPLOY SCRIPT TO EXECUTE ON BUILD SO ENV GETS POPULATED
// Define the path to the environment file
const targetPath = path.join(__dirname, './src/environments/environment.prod.ts');

// Environment file content, ensuring the variable names match your Vercel setup
const envConfigFile = `export const environment = {
  production: true,
  apiUrl: '${process.env.apiUrl}',
  stripePublishKey: '${process.env.stripePublishKey}',
    adminCredentials: {
    email: '${process.env.adminCredentials.email}',
    password: '${process.env.adminCredentials.password}',
  },
  moderatorCredentials: {
    email: '${process.env.moderatorCredentials.email}',
    password: '${process.env.moderatorCredentials.password}',
  },
};
`;

// Write the environment file
fs.writeFile(targetPath, envConfigFile, 'utf8', (err) => {
  if (err) {
    return console.log(err);
  }

  console.log(`Environment variables were successfully written to ${targetPath}`);
});
