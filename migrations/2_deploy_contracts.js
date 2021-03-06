/*const NFT = artifacts.require("NFT");

module.exports = function (deployer) {
    deployer.deploy(NFT);
};*/

const { ethers } = require('hardhat')

module.exports = async ({
    getNamedAccounts,
    deployments,
}) => {
    const { deploy } = deployments
    const { deployer } = await getNamedAccounts()

    await deploy('NFT', {
        from: deployer,
        log: true,
    })
}
module.exports.tags = ['NFT']
