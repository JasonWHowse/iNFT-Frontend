// SPDX-License-Identifier: MIT
pragma solidity ^0.8.0;
// pragma abicoder v2; // required to accept structs as function parameters

import '@openzeppelin/contracts/token/ERC721/extensions/ERC721URIStorage.sol';
import './INFT.sol';
import '@openzeppelin/contracts/utils/Counters.sol';

contract NFT is ERC721URIStorage, INFT {
  using Counters for Counters.Counter;
  Counters.Counter private _tokenIds; // Counter for tokenIds

  /**
   * @dev Constructor for name and symbol.
   */
  constructor() ERC721('iNFT', 'iNFT') {}

  /**
   * @dev Base URI for computing {tokenURI}. The resulting URI for each
   * token will be the concatenation of the `baseURI` and the `tokenId`.
   */
  function _baseURI()
    internal
    view
    virtual
    override
    returns (string memory)
  {
    return 'https://ipfs.io/ipfs/';
  }

  function mint(
    address to,
    string memory tokenURI
  ) public override returns (uint256) {
    _tokenIds.increment(); // Increment counter
    uint256 newTokenId = _tokenIds.current(); // Create a tokenId from counter
    _mint(to, newTokenId); // Mint token
    _setTokenURI(newTokenId, tokenURI); // Save tokenURI
    return newTokenId; // Return tokenId
  }
}