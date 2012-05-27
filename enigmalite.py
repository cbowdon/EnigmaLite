# This is a proof of concept / tutorial 
# for doing word frequency matching and letter substitution.
# There are a few bad practices in this code,
# and a lot of potential bugs,
# but it should be highly readable to new programmer.

# ALL THE FUNCTION DEFINITIONS

def get_text (filename):
    """Get lines of text from file"""
    with open(filename) as input:
        lines = [line for line in input]
    return lines

def get_words (lines):
    """Splits each line of text into words 
    and joins each word onto one long list"""
    
    # get a list of  all the lines, split by spaces
    split_lines = [line.split(" ") for line in lines]
    
    # turn this into a list of words
    words = []
    for x in split_lines:
        words.extend(x)
        
    # output is the list of words
    return words

def replace_letter (text, old_letter, new_letter):
    """Replace letter in every line of text"""
    edited = [line.replace(old_letter, new_letter) for line in text]
    return edited

def frequency_sort (word_list):
    """Sorts a list of words by frequency"""
    
    # dictionary of frequencies for each word (empty to start with)
    freqs = dict()
    # go through the word list, word by word:
    # add one to the tally for each word
    # if a word is not in the dictionary, 
    # add it and give it a count of one
    for w in word_list:
        if w in freqs:
            freqs[w] += 1
        else:
            freqs[w] = 1
    
    # sort the words by their frequency
    sorted_words = sorted(freqs, key=freqs.get)

    # it sorts from low to high, so reverse it 
    sorted_words.reverse()

    return sorted_words

def find_likely (a_word):
    """Find the likeliest match that is a real word"""

    # this list is just for a test
    # top word for each number of letters
    top_words = ["a", "it", "the", "herp", "aderp", "batman", "bananas"]
    
    # we're just looking for the same number of letters
    num_letters = len(a_word)

    # remember that list indices start from zero
    return top_words[num_letters-1] 


# TEST THE FUNCTIONS

my_word_list = ("hi", "how", "are", "you", "sonny", "jim", "jim", "jim", "you", "sonny", "you", "jim")

# sort the words by frequency
my_sorted_words =  frequency_sort (my_word_list)

# get the most frequent words (english & cryptic)
top_cryptic_word =  my_sorted_words[0]
top_english_word = find_likely (my_sorted_words[0])


# create a dictionary of letters and their match
cypher = dict()

# add to dictionary
for i in range(len(top_cryptic_word)):
    cypher[top_cryptic_word[i]] = top_english_word[i]

# print the cypher so far
# check that it has successfully matched
# "t" to "j"
# "h" to "i"
# "e" to "m"
# what order they are in doesn't matter, as long as they match
for i in cypher:
    print i, cypher[i]
