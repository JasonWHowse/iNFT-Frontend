// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;

import '@openzeppelin/contracts/token/ERC721/IERC721.sol';
import '@openzeppelin/contracts/utils/Counters.sol';

interface INFT is IERC721 {
  /**
   * @dev Mint a new token that stores the tokenURI
   *
   * @param to Address to the token owner
   * @param tokenURI The token metedata
   */
  function mint(
    address to,
    string memory tokenURI
  ) external returns (uint256);
}
